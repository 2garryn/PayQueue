using System;
using PayQueue.Internal;
using PayQueue.QueueInterfaces;

namespace PayQueue.Publishing
{
    using MessageType = Type;
    internal class Factory
    {
        private Command.Catalog _commandCatalog;
        private Event.Catalog _eventCatalog;
        private DepsCatalog _deps;
        private readonly string _sourceService;
        private readonly string _sourceHost;
        public Factory(Command.Catalog commandCatalog, Event.Catalog eventCatalog, DepsCatalog deps, string sourceService, string sourceHost)
        {
            _commandCatalog = commandCatalog;
            _eventCatalog = eventCatalog;
            _deps = deps;
            _sourceService = sourceService;
            _sourceHost = sourceHost;
        }

        public Publisher New(IExchangePublisher publisher) => 
            new Publisher(publisher, _commandCatalog, _eventCatalog, _deps, _sourceService, _sourceHost);
        
    }
}