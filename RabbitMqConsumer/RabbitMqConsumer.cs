using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Channels;
using System.Text.Json;
using PayQueue.QueueInterfaces;

namespace PayQueue.RabbitMqConsumer
{

    internal class RabbitMqConsumer : IQueueConsumer
    {
        private readonly IConnection _conn;
       // private readonly Channel<PublishMessage> _poolChannel;
       // private Task _poolTask;
        private RabbitConfiguration _conf;
        private PooledExchangePublisher _poolPublisher;

        public RabbitMqConsumer(RabbitConfiguration conf)
        {
            _conf = conf;
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = _conf.Username,
                Password = _conf.Password,
                VirtualHost = _conf.VHost,
                HostName = _conf.Hostname,
                DispatchConsumersAsync = true
            };
            _conn = factory.CreateConnection();
            _poolPublisher = new PooledExchangePublisher(_conn, 15);
        }

        public async Task Start() => await _poolPublisher.Start();
        


        public void RegisterCommandConsumer(string queue, IConsumeExecutor executor)
        {
            var channel = _conn.CreateModel();
            channel.ExchangeDeclare(queue, ExchangeType.Direct, true, false);
            channel.QueueDeclare(queue, true, false, false, null);
            channel.QueueBind(queue, queue, "", null);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += GetReceived(executor, queue, channel);
            channel.BasicConsume(queue, false, consumer);
        }
        public void RegisterEventConsumer(string queue, string[] exchanges, IConsumeExecutor executor)
        {
            var channel = _conn.CreateModel();
            channel.QueueDeclare(queue, true, false, false, null);
            foreach (var exchange in exchanges)
            {
                channel.ExchangeDeclare(exchange, ExchangeType.Fanout, true, false);
                channel.QueueBind(queue, exchange, "", null);
            }
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += GetReceived(executor, queue, channel);
            channel.BasicConsume(queue, false, consumer);
        }

        public IExchangePublisher GetExchangePublisher() => _poolPublisher;


        private AsyncEventHandler<BasicDeliverEventArgs> GetReceived(IConsumeExecutor executor, string queue, IModel channel)
        {
            return async (model, ea) =>
            {
                var metadata = new ConsumeMessageMetadata()
                    {
                        Queue = queue,
                        Exchange = ea.Exchange

                    };
                Func<string> messageType = () => (string)ea.BasicProperties.Headers["message_type"];
                try
                {

                    await executor.Execute(GetExchangePublisher(), messageType, ea.Body.ToArray(), metadata);
                    channel.BasicAck(ea.DeliveryTag, false);
                    await Task.Yield();
                }
                catch (Exception e)
                {
                    var dataArr = ea.Body.ToArray();
                    channel.BasicNack(ea.DeliveryTag, false, true);
                    var errMsg = new PayQueueError()
                    {
                        ServiceLabel = _conf.ServiceLabel,
                        ConsumeMessageType = messageType(),
                        ConsumeParams = Encoding.UTF8.GetString(dataArr, 0, dataArr.Length),
                        ExceptionType = e.GetType().ToString(),
                        ExceptionMessage = e.Message,
                        ExceptionStacktrace = e.StackTrace,
                        Exchange = ea.Exchange
                    };
                    var data = JsonSerializer.Serialize(errMsg);
                    await _poolPublisher.PublishError(_conf.ErrorExchange, _conf.ErrorQueue, Encoding.UTF8.GetBytes(data));
                        
                }
                
            };
        }
        public async Task Stop() 
        {
            await Task.Yield();
        }
    }



}