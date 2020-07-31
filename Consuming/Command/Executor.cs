using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PayQueue.Consuming;
using PayQueue.Internal;
using PayQueue.QueueInterfaces;

namespace PayQueue.Consuming.Command
{
    internal class Executor<T>: IExecutor
    {
        private ConsumerFactory<T> _consumerFactory;
        private DepsCatalog _deps;
        private MessageContextFactory _contextFactory;
        internal Executor(ConsumerFactory<T> consumerFactory, MessageContextFactory contextFactory, DepsCatalog deps)
        {
            _consumerFactory = consumerFactory;
            _deps = deps;
            _contextFactory = contextFactory;
        }

        public async Task Execute(IExchangePublisher exchangePublisher, byte[] data)
        {
            var messageContext = _contextFactory.New<T>(exchangePublisher, data);
            var impl = _consumerFactory.New();
            _deps.Logger.LogDebug($"Consume command {typeof(T).FullName}. RequestID: {messageContext.RequestId} ConversationID: {messageContext.ConversationId} SourceService: {messageContext.SourceService} PublishTimestamp: {messageContext.PublishTimestamp}");
            await _deps.MiddlewareCommand().Invoke(messageContext, impl.ConsumeCommand);
            _deps.Logger.LogDebug($"Consumed command {typeof(T).FullName} successfully. RequestID: {messageContext.RequestId}");
        }
    }
}