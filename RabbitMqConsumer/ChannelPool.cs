using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Threading.Channels;

namespace PayQueue.RabbitMqConsumer
{

    public class ChannelPool
    {
        private readonly IConnection _connection;
        private readonly int _size;
        private readonly ChannelReader<PublishMessage> _chan;
        public ChannelPool(IConnection connection, int size, ChannelReader<PublishMessage> chan)
        {
            _size = size;
            _connection = connection;
            _chan = chan;
        }

        public async Task Run()
        {
            Task[] tasks = new Task[_size];
            for (var i = 0; i < _size; i++)
            {
                tasks[i] = new Task(async () => await RunWorker(), TaskCreationOptions.LongRunning);
                tasks[i].Start();
            }
            await Task.Yield();
        }
        public async Task RunWorker()
        {
            var exchanges = new HashSet<string>();
            var model = _connection.CreateModel();
            while (await _chan.WaitToReadAsync())
            {
                if (_chan.TryRead(out var tsk))
                {
                    if (!exchanges.Contains(tsk.ExchangeName))
                    {
                        model.ExchangeDeclare(tsk.ExchangeName, tsk.ExchangeType, true, false);
                        model.QueueDeclare(tsk.QueueName, true, false, false, null);
                        model.QueueBind(tsk.QueueName, tsk.ExchangeName, "", null);
                        exchanges.Add(tsk.ExchangeName);
                    }
                    var props = model.CreateBasicProperties();
                    tsk.Properties(props);
                    
                    model.BasicPublish(tsk.ExchangeName, tsk.RoutingKey, true, props, tsk.Body);
                    model.WaitForConfirms();
                }
            }
        }
    }

}