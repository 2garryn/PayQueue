using System;
using System.Collections.Generic;
using System.Linq;
using PayQueue.Definition;
using PayQueue.Impl;
using Microsoft.Extensions.Logging;
using PayQueue.Consuming;
using PayQueue.Internal;
using PayQueue.QueueInterfaces;

namespace PayQueue
{

    public class PayQueueCreator<TServDef, TImpl>
        where TServDef : IServiceDefinition, new()
        where TImpl : IServiceImpl<TServDef>
    {
        private readonly IQueueConsumer _consumer;
        private  IConsumerImplFactory _consumerImplFactory;
        private  IImplFactory<TImpl> _implFactory;
        private string _prefix = "PayQueue";
        private Func<IMiddlewareCommand> _commandMiddleware = () => new DefaultMiddleware();
        private Func<IMiddlewareEvent> _eventMiddleware = () => new DefaultMiddleware();
        private ILogger _logger;

        internal PayQueueCreator(IQueueConsumer queueConsumer)
        {

            _consumer = queueConsumer;
        }

        public PayQueueCreator<TServDef, TImpl> Factory(TImpl single)
        {
            _consumerImplFactory = new Consuming.Factories.Singleton<TImpl>(single);
            _implFactory = new ImplfactorySingleton<TImpl>(single);
            return this;
        }
        public PayQueueCreator<TServDef, TImpl> Factory(Func<TImpl> deleg)
        {
            _consumerImplFactory = new Consuming.Factories.Delegate<TImpl>(deleg);
            _implFactory = new ImplfactoryDelegate<TImpl>(deleg);
            return this;
        }

        public PayQueueCreator<TServDef, TImpl> SetPrefix(string prefix)
        {
            _prefix = prefix;
            return this;
        }
        public PayQueueCreator<TServDef, TImpl> UseCommandMiddleware(Func<IMiddlewareCommand> middleware) 
        {
            _commandMiddleware = middleware;
            return this;
        }
        public PayQueueCreator<TServDef, TImpl> UseEventMiddleware(Func<IMiddlewareEvent> middleware) 
        {
            _eventMiddleware = middleware;
            return this;
        }
        public PayQueueCreator<TServDef, TImpl> UseLogger(ILogger logger) 
        {
            _logger = logger;
            return this;
        }

        public PayQueueService<TServDef, TImpl> Build()
        {
            var deps = new DepsCatalog()
            {
                Prefix = _prefix,
                MiddlewareCommand = _commandMiddleware,
                MiddlewareEvent = _eventMiddleware,
                Logger = _logger ?? GetDefaultLogger()
            };
            return new PayQueueService<TServDef, TImpl>() 
            {
                QueueConsumer = _consumer,
                DepsCatalog = deps,
                ConsumerImplFactory = _consumerImplFactory,
                ImplFactory = _implFactory
            };
        }

        private ILogger GetDefaultLogger()
        {
            var defType = typeof(TServDef).Name;
            var implType = typeof(TImpl).Name;
            var category = $"[{defType}: {implType}]";
            return LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Debug);
                })
                .CreateLogger(category);
        }
  
    }


}