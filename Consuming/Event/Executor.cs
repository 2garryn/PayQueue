using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PayQueue.Internal;
using PayQueue.Definition;
using PayQueue.QueueInterfaces;
using PayQueue.Impl;

namespace PayQueue.Consuming.Event
{
    internal class Executor<TServiceDefinition, T>: IExecutor where TServiceDefinition : IServiceDefinition, new()
    {
        private readonly ConsumerFactory<TServiceDefinition, T> _consumerFactory;
        private readonly DepsCatalog _deps;
        private readonly MessageContextFactory _contextFactory;
        internal Executor(ConsumerFactory<TServiceDefinition, T> consumerFactory, MessageContextFactory contextFactory, DepsCatalog deps)
        {
            _consumerFactory = consumerFactory;
            _deps = deps;
            _contextFactory = contextFactory;
        }
        
        public async Task Execute(IExchangePublisher exchangePublisher, byte[] data)
        {
            var messageContext = _contextFactory.New<T>(exchangePublisher, data);
            var impl = _consumerFactory.New();
            _deps.Logger.LogDebug($"Consume event {typeof(T).FullName}. RequestID: {messageContext.RequestId} ConversationID: {messageContext.ConversationId} SourceService: {messageContext.SourceService} PublishTimestamp: {messageContext.PublishTimestamp}");
            await _deps.MiddlewareEvent().Invoke(messageContext, impl.ConsumeEvent);
            _deps.Logger.LogDebug($"Consumed event {typeof(T).FullName} successfully. RequestID: {messageContext.RequestId}");
        }
    }

}