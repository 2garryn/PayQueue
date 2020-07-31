using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using PayQueue.Internal;
using PayQueue.QueueInterfaces;

namespace PayQueue.Consuming.Command
{
    internal class Builder
    {
        private Dictionary<string, IExecutor> _catalog;
        private readonly MessageContextFactory _contextFactory;
        private readonly Routes _routes;
        private readonly DepsCatalog _deps;
        public Builder(Routes routes, MessageContextFactory contextFactory, DepsCatalog deps)
        {
            _catalog = new Dictionary<string, IExecutor>();
            _routes = routes;
            _contextFactory = contextFactory;
            _deps = deps;
        }

        public void Consume<T>(IExecutor executor) =>
            _catalog[typeof(T).FullName] = executor;

        public IConsumeExecutor GetExecuter() =>
            new CatalogExecutor(_catalog.ToImmutableDictionary(), _contextFactory, _deps);

        public Route GetConsumeRoute() => new Route(_routes, _catalog.Count > 0);

    }
}