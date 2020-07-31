using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayQueue.Internal;
using PayQueue.Consuming;
using PayQueue.Definition;
using PayQueue.Impl;
using PayQueue.QueueInterfaces;

namespace PayQueue
{

    public class PayQueueService<TServDef, TImpl>
        where TServDef : IServiceDefinition, new()
        where TImpl : IServiceImpl<TServDef>
    {
        internal IQueueConsumer QueueConsumer {get; set;}
        internal DepsCatalog DepsCatalog {get;set;}
        internal IConsumerImplFactory ConsumerImplFactory {get;set;}
        internal IImplFactory<TImpl> ImplFactory {get;set;}
        private Publishing.Factory _publisherFactory;

        public async Task<TResult> CallAsync<TResult>(Func<TImpl, IPublisher, Task<TResult>> caller)
        {
            var publ = _publisherFactory.New(QueueConsumer.GetExchangePublisher());
            return await caller(ImplFactory.New(), publ);
        }
        
        public async Task CallAsync(Func<TImpl, IPublisher, Task> caller)
        {
            var publ = _publisherFactory.New(QueueConsumer.GetExchangePublisher());
            await caller(ImplFactory.New(), publ);
        }

        public void Run()
        {
            var def = new TServDef();
            var routes = new Routes(DepsCatalog.Prefix, def.Label());
            var publFactoryBuilder = new Publishing.FactoryBuilder(routes, DepsCatalog, def.Label(), "none");
            def.Configure(new ExecuteConfigurator(null, publFactoryBuilder));
            _publisherFactory = publFactoryBuilder.Build();
            var contextFactory = new MessageContextFactory(_publisherFactory);
            var consumeCatalog = new Consuming.CatalogBuilder(routes, contextFactory, ConsumerImplFactory, DepsCatalog);
            def.Configure(new ExecuteConfigurator(consumeCatalog, null));
            consumeCatalog.Register(QueueConsumer);
        }

        public async Task Stop()
        {
            await QueueConsumer.Stop();
        }

    }


}