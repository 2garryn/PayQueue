using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using PayQueue.Exceptions;
using PayQueue.Publishing;
using PayQueue.QueueInterfaces;
using PayQueue.Impl;

namespace PayQueue.Internal
{

    internal class MessageContextFactory
    {
        private readonly Factory _publisherFactory;
        internal MessageContextFactory(Factory publisherFactory)
        {
            _publisherFactory = publisherFactory;
        }

        public MessageContext<T> New<T>(IExchangePublisher expPubl, byte[] data)
        {
            var datagram = Deserialize<T>(data);
            return new MessageContext<T>()
            {
                RequestId = datagram.RequestId,
                Message = datagram.Message,
                SourceHost = datagram.SourceHost,
                SourceService = datagram.SourceService,
                PublishTimestamp = datagram.PublishTimestamp,
                ConversationId = datagram.ConversationId,
                Publisher = _publisherFactory.New(expPubl)
            };
        }

        private DatagramMessage<T> Deserialize<T>(byte[] data)
        {
            try
            {
                return JsonSerializer.Deserialize<DatagramMessage<T>>(data);
            }
            catch (ArgumentNullException inner)
            {
                var excp = new PayQueueException("Can not deserialize message", inner);
                excp.Data["Type"] = typeof(T);
                throw excp;
            }
            catch (JsonException inner)
            {
                var excp = new PayQueueException("Can not deserialize message", inner);
                excp.Data["Type"] = typeof(T);
                throw excp;
            }
        }

    }


}