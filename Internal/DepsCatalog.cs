using System;
using Microsoft.Extensions.Logging;
using PayQueue.Definition;

namespace PayQueue.Internal
{
    internal class DepsCatalog
    {
        public string Prefix { get; internal set;}
        public Func<IMiddlewareCommand> MiddlewareCommand { get; internal set;}
        public Func<IMiddlewareEvent> MiddlewareEvent { get; internal set;}
        public ILogger Logger { get; internal set; }
    }
}