using System;
using Microsoft.Extensions.Logging;
using PayQueue.Internal;
using PayQueue.Definition;

namespace PayQueue.Publishing
{
    internal class FactoryBuilder
    {
        private Command.Builder _commandCatalog;
        private Event.Builder _eventCatalog;
        private DepsCatalog _deps;
        private readonly string _sourceService;
        private readonly string _sourceHost;
        public FactoryBuilder(Routes routes, DepsCatalog depsCatalog, string sourceService, string sourceHost)
        {
            _eventCatalog = new Event.Builder(routes, depsCatalog);
            _commandCatalog = new Command.Builder(routes, depsCatalog);
            _deps = depsCatalog;
            _sourceService = sourceService;
            _sourceHost = sourceHost;
        }

        public void Command<S, T>() where S : IServiceDefinition, new() => _commandCatalog.Command<S, T>();
        public void PublishEvent<T>() => _eventCatalog.PublishEvent<T>();
        public void PublishEvent<T>(Func<T, string> route) => _eventCatalog.PublishEvent<T>(route);

        public Factory Build()
        {
            return new Factory(_commandCatalog.Build(), _eventCatalog.Build(), _deps, _sourceService, _sourceHost);
        }
    }
}