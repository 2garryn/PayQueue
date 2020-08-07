using System;
using System.Collections.Generic;
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
        public ConnectedChannel(IConnection connection)
        {
            _exchanges = new HashSet<string>();
            _model = connection.CreateModel();
        }
        public async Task<PublishResult> Publish(PublishMessage tsk)
        {
            if (!_exchanges.Contains(tsk.ExchangeName))
            {
                _model.ExchangeDeclare(tsk.ExchangeName, tsk.ExchangeType, true, false);
                _model.QueueDeclare(tsk.QueueName, true, false, false, null);
                _model.QueueBind(tsk.QueueName, tsk.ExchangeName, "", null);
                _exchanges.Add(tsk.ExchangeName);
            }
            var props = _model.CreateBasicProperties();
            tsk.Properties(props);

            _model.BasicPublish(tsk.ExchangeName, tsk.RoutingKey, true, props, tsk.Body);
            _model.WaitForConfirms();
        }

    }

}