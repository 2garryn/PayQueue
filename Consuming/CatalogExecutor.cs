using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Text.Json;
using PayQueue.Exceptions;
using PayQueue.Internal;
using PayQueue.QueueInterfaces;


namespace PayQueue.Consuming
{
    internal class CatalogExecutor : IConsumeExecutor
    {
        private readonly ImmutableDictionary<string, IExecutor> _consumers;
        private readonly MessageContextFactory _contextFactory;
        private readonly DepsCatalog _deps;
        public CatalogExecutor(ImmutableDictionary<string, IExecutor> consumers, MessageContextFactory contextFactory, DepsCatalog depsCatalog)
        {
            _consumers = consumers;
            _contextFactory = contextFactory;
            _deps = depsCatalog;
        }
        
        public async Task Execute(IExchangePublisher exchangePublisher, Func<string> messageType, byte[] data, ConsumeMessageMetadata metadata)
        {
            await GetExecutor(messageType(), metadata).Execute(exchangePublisher, data);
        }
        

        private IExecutor GetExecutor(string messageType, ConsumeMessageMetadata metadata)
        {
            try
            {
                return _consumers[messageType];
            }
            catch (KeyNotFoundException)
            {
                var excp = new PayQueueException("Received message was not defined in contract.");
                excp.Data["Queue"] = metadata.Queue;
                excp.Data["Exchange"] = metadata.Exchange;
                excp.Data["Type"] = messageType;
                throw excp;
            }
        }
        

    }


}