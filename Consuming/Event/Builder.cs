using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using PayQueue.Consuming;
using PayQueue.Internal;
using PayQueue.Definition;
using PayQueue.QueueInterfaces;
using PayQueue;

namespace PayQueue.Consuming.Event
{
    internal class Builder
    {
        private Dictionary<string, IExecutor> _catalog;
        private readonly Route _consumeRoutes;
        private readonly MessageContextFactory _contextFactory;
        private readonly DepsCatalog _deps;
        public Builder(Routes routes, MessageContextFactory contextFactory, DepsCatalog deps)
        {
            _catalog = new Dictionary<string, IExecutor>();
            _consumeRoutes = new Route(routes);
            _contextFactory = contextFactory;
            _deps = deps;
        }

        public void Consume<S, T>(IExecutor executor) 
            where S : IServiceDefinition, new()
        {
            _catalog[typeof(T).FullName] = executor;
            _consumeRoutes.Add<S, T>();
        }
        public void Consume<S, T>(string routeKey, IExecutor executor) 
            where S : IServiceDefinition, new()
        {
            _catalog[typeof(T).FullName] = executor;
            _consumeRoutes.Add<S, T>(routeKey);
        }

        public IConsumeExecutor GetExecuter() =>
            new CatalogExecutor(_catalog.ToImmutableDictionary(), _contextFactory, _deps);

        public Route GetConsumeRoute() =>
            _consumeRoutes;
    }
}