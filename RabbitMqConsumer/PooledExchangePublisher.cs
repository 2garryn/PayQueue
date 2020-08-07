using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Threading.Channels;
using System.Collections.Generic;
using PayQueue.QueueInterfaces;
using PayQueue.Impl;
namespace PayQueue.RabbitMqConsumer
{

    public class PooledExchangePublisher : IExchangePublisher
    {
        private ObjectPoolPublisher _poolPublisher;
        public PooledExchangePublisher(IConnection connection, int size)
        {
            _poolPublisher = new ObjectPoolPublisher(connection, 15);
        }
        public async Task Start() => await _poolPublisher.Start();

        public async Task<PublishResult> PublishEvent(string exchange, string messageType, byte[] data) =>
            await _poolPublisher.Publish(new PublishMessage
            {
                Properties = (props) => 
                {
                    props.ContentType = "application/json";
                    props.Headers = new Dictionary<string, object>();
                    props.Headers.Add("message_type", messageType);
                },
                Body = data,
                ExchangeName = exchange,
                ExchangeType = ExchangeType.Fanout,
                CreateQueue = false,
                RoutingKey = ""
            });
        
        public async Task<PublishResult> Command(string exchange, string messageType, byte[] data) =>
            await _poolPublisher.Publish(new PublishMessage
            {
                Properties = (props) => 
                {
                    props.ContentType = "application/json";
                    props.Headers = new Dictionary<string, object>();
                    props.Headers.Add("message_type", messageType);
                },
                Body = data,
                ExchangeName = exchange,
                ExchangeType = ExchangeType.Direct,
                QueueName = exchange,
                CreateQueue = true,
                RoutingKey = ""
            });
        

        public async Task PublishError(string exchange, string queue, byte[] data) =>
            await _poolPublisher.Publish(new PublishMessage
            {
                Properties = (props) => 
                {
                    props.ContentType = "application/json";
                },
                Body = data,
                ExchangeName = exchange,
                ExchangeType = ExchangeType.Direct,
                QueueName = queue,
                CreateQueue = true,
                RoutingKey = ""
            });
        

    }


}