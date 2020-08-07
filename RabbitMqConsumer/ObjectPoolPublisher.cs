using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Threading.Tasks.Dataflow;
using PayQueue.Impl;

namespace PayQueue.RabbitMqConsumer
{

    public class ObjectPoolPublisher
    {

        private readonly IConnection _connection;
        private readonly int _size;
        private readonly BufferBlock<ConnectedChannel> _buffer;

        public ObjectPoolPublisher(IConnection connection, int size)
        {
            _size = size;
            _connection = connection;
            _buffer = new BufferBlock<ConnectedChannel>();
        }

        public async Task Start()
        {
            for (var i = 0; i < _size; i++)
            {
                await _buffer.SendAsync(new ConnectedChannel(_connection));
            }
        }

        public async Task<PublishResult> Publish(PublishMessage message)
        {
            var chan = await _buffer.ReceiveAsync();
            var result = await chan.Publish(message);
            await _buffer.SendAsync(chan);
            return result;
        }
    }


    internal class ConnectedChannel
    {
        private HashSet<string> _exchanges;
        private IModel _model;
        private ConcurrentDictionary<ulong, TaskCompletionSource<PublishResult>> _confirmResults;
        public ConnectedChannel(IConnection connection)
        {
            _exchanges = new HashSet<string>();
            _confirmResults = new ConcurrentDictionary<ulong, TaskCompletionSource<PublishResult>>();
            _model = connection.CreateModel();
            _model.ConfirmSelect();
            _model.BasicAcks += (sender, ea) =>
            {
            // code when message is confirmed
                if(_confirmResults.TryRemove(ea.DeliveryTag, out TaskCompletionSource<PublishResult> taskSource)) 
                {
                    taskSource.TrySetResult(new PublishResult{PublishStatus = PublishStatus.Published});
                    
                }
            };
            _model.BasicNacks += (sender, ea) =>
            {
                if(_confirmResults.TryRemove(ea.DeliveryTag, out TaskCompletionSource<PublishResult> taskSource)) 
                {
                    taskSource.TrySetResult(new PublishResult{PublishStatus = PublishStatus.NoDestination});
                }
            };
   
        }
        public async Task<PublishResult> Publish(PublishMessage tsk)
        {
            var tcs = new TaskCompletionSource<PublishResult>();
            _confirmResults.TryAdd(_model.NextPublishSeqNo, tcs);

            if (!_exchanges.Contains(tsk.ExchangeName))
            {
                _model.ExchangeDeclare(tsk.ExchangeName, tsk.ExchangeType, true, false);
                if (tsk.CreateQueue) 
                {
                    _model.QueueDeclare(tsk.QueueName, true, false, false, null);
                    _model.QueueBind(tsk.QueueName, tsk.ExchangeName, "", null);
                }
                _exchanges.Add(tsk.ExchangeName);
            }
            var props = _model.CreateBasicProperties();
            tsk.Properties(props);

            _model.BasicPublish(tsk.ExchangeName, tsk.RoutingKey, true, props, tsk.Body);
            return await tcs.Task;
        }

    }

}