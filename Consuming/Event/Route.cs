using System;
using System.Collections.Generic;
using System.Linq;
using PayQueue.Definition;
using PayQueue.Internal;

namespace PayQueue.Consuming.Event
{
    internal class Route
    {
        private Routes _routes;
        private HashSet<string> _exchanges;
        public Route(Routes routes)
        {
            _routes = routes;
            _exchanges = new HashSet<string>();
        }
        
        public void Add<S, T>( string routeKey = null) where S : IServiceDefinition, new() =>
            _exchanges.Add(_routes.ConsumeEventExchange(new S().Label(), typeof(T), routeKey));
        public string Queue
        {
            get { return _routes.ConsumeEventQueue(); }
        }
        public string[] Exchanges
        {
            get { return _exchanges.ToArray();  }
        }
        public bool IsApplicable
        {
            get { return _exchanges.Count() > 0; }
        }
    }
}