using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using PayQueue.Exceptions;

namespace PayQueue.Publishing.Command
{
    public class Catalog
    {
        private  ImmutableDictionary<Type, ImmutableDictionary<Type, string>> _catalog;

        public Catalog(ImmutableDictionary<Type, ImmutableDictionary<Type, string>> catalog)
        {
            _catalog = catalog;
        }

        public string GetRoute<S, T>()
        {
            try
            {
                return _catalog[typeof(S)][typeof(T)];
            }
            catch (KeyNotFoundException)
            {
                var excp = new PayQueueException("Can not find publish command");
                excp.Data["Service"] = typeof(S).FullName;
                excp.Data["Command"] = typeof(T).FullName;
                throw excp;
            }
        }
    }
}