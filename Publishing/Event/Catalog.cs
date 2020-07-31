using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using PayQueue.Exceptions;

namespace PayQueue.Publishing.Event
{
    public class Catalog
    {
        private  ImmutableDictionary<Type, Func<object, string>> _catalog;

        public Catalog(ImmutableDictionary<Type, Func<object, string>> catalog)
        {
            _catalog = catalog;
        }

        public string GetRoute<T>(T message) => Route<T>()(message);
        

        private Func<object, string> Route<T>()
        {
            try
            {
                return _catalog[typeof(T)];
            }
            catch (KeyNotFoundException)
            {
                var excp = new PayQueueException("Can not find publish event");
                excp.Data["Event"] = typeof(T).FullName;
                throw excp;
            }
        }

    }
}