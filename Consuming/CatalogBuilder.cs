using System;
using Microsoft.Extensions.Logging;
using PayQueue;
using PayQueue.Internal;
using PayQueue.Definition;
using PayQueue.QueueInterfaces;

namespace PayQueue.Consuming
{
    internal class CatalogBuilder
    {
        private Command.Builder _commandCatalog;
        private Event.Builder _eventCatalog;
        private IConsumerImplFactory _consumerFactory;
        private MessageContextFactory _contextFactory;
        private readonly DepsCatalog _deps;


        public CatalogBuilder(Routes routes, MessageContextFactory contextFactory, IConsumerImplFactory consumerFactory, DepsCatalog deps)
        {
            _consumerFactory = consumerFactory;
            _contextFactory = contextFactory;
            _deps = deps;
            _commandCatalog = new Command.Builder(routes, contextFactory, _deps);
            _eventCatalog = new Event.Builder(routes, contextFactory, _deps);
        }


        public void ConsumeCommand<T>()
        {
            _deps.Logger.LogDebug($"Define consume command {typeof(T).FullName}");
            var consumerFactory = _consumerFactory.NewCommandConsumerFactory<T>();
            _commandCatalog.Consume<T>(new Command.Executor<T>(consumerFactory, _contextFactory, _deps));
        }


        public void ConsumeEvent<TServDef, T>() where TServDef : IServiceDefinition, new()  
        {
            _deps.Logger.LogDebug($"Define consume event {typeof(TServDef).FullName}:{typeof(T).FullName}");
            var consumerFactory = _consumerFactory.NewEventConsumerFactory<TServDef, T>();
            _eventCatalog.Consume<TServDef, T>(new Event.Executor<TServDef, T>(consumerFactory, _contextFactory, _deps)); 
        }


        public void ConsumeEvent<S, T>(string key) where S : IServiceDefinition, new()
        {
            _deps.Logger.LogDebug($"Define consume event {typeof(S).FullName}:{typeof(T).FullName} with route key {key}");
            var consumerFactory = _consumerFactory.NewEventConsumerFactory<S, T>();
            _eventCatalog.Consume<S, T>(key, new Event.Executor<S, T>(consumerFactory, _contextFactory, _deps));
        }
            

        public void Register(IQueueConsumer qConsumer)
        {
            RegisterEvents(qConsumer);
            RegisterCommands(qConsumer);
        }

        private void RegisterCommands(IQueueConsumer qConsumer)
        {
            if (!_commandCatalog.GetConsumeRoute().IsApplicable)
                return;
            _deps.Logger.LogDebug($"Register command queue: {_commandCatalog.GetConsumeRoute().Queue}");
            qConsumer.RegisterCommandConsumer(_commandCatalog.GetConsumeRoute().Queue, _commandCatalog.GetExecuter());
        }
        private void RegisterEvents(IQueueConsumer qConsumer)
        {
            var consumeRoutes = _eventCatalog.GetConsumeRoute();
            if (!consumeRoutes.IsApplicable)
                return;
            var joined = String.Join(", ", consumeRoutes.Exchanges);
            _deps.Logger.LogDebug($"Register event queue: {consumeRoutes.Queue}, exchanges: {joined}");
            qConsumer.RegisterEventConsumer(consumeRoutes.Queue, consumeRoutes.Exchanges, _eventCatalog.GetExecuter());
        }
    }
}