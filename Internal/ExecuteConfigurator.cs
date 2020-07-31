using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PayQueue.Definition;

namespace PayQueue.Internal
{

    internal class ExecuteConfigurator : IConfigurator
    {
        //private readonly CallbackExecutorFactory _execFactory;
        private readonly Consuming.CatalogBuilder _consumeCatalog;
        private readonly Publishing.FactoryBuilder _publishCatalog;

        public ExecuteConfigurator(Consuming.CatalogBuilder consumeCatalog, Publishing.FactoryBuilder publishCatalog) => 
             (_consumeCatalog, _publishCatalog) = (consumeCatalog, publishCatalog);

        public void ConsumeCommand<T>() =>_consumeCatalog?.ConsumeCommand<T>();
        
        public void ConsumeEvent<S, T>() where S : IServiceDefinition, new() =>
            _consumeCatalog?.ConsumeEvent<S, T>();
        public void ConsumeEvent<S, T>(string key) where S : IServiceDefinition, new() =>
            _consumeCatalog?.ConsumeEvent<S, T>(key);
        public void Command<S, T>() where S : IServiceDefinition, new() =>  _publishCatalog?.Command<S, T>();

        public void PublishEvent<T>() => _publishCatalog?.PublishEvent<T>();

        public void PublishEvent<T>(Func<T, string> routeFormatter) => _publishCatalog?.PublishEvent<T>(routeFormatter);

    }
}