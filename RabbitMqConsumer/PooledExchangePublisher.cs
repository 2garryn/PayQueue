using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Threading.Channels;
using System.Collections.Generic;
using PayQueue.QueueInterfaces;

namespace PayQueue.RabbitMqConsumer
{

    public class PooledExchangePublisher : IExchangePublisher
    {
        private readonly ChannelWriter<PublishMessage> _channel;
        public PooledExchangePublisher(ChannelWriter<PublishMessage> channel)
        {
            _channel = channel;
        }

        public async Task PublishEvent(string exchange, string messageType, byte[] data)
        {          
            await _channel.WriteAsync(new PublishMessage
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
                QueueName = exchange,
                RoutingKey = ""
            });
        }
        public async Task Command(string exchange, string messageType, byte[] data)
        {
            await _channel.WriteAsync(new PublishMessage
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
                RoutingKey = ""
            });
        }

        public async Task PublishError(string exchange, string queue, byte[] data)
        {
            await _channel.WriteAsync(new PublishMessage
            {
                Properties = (props) => 
                {
                    props.ContentType = "application/json";
                },
                Body = data,
                ExchangeName = exchange,
                ExchangeType = ExchangeType.Direct,
                QueueName = exchange,
                RoutingKey = ""
            });
        }

    }


}