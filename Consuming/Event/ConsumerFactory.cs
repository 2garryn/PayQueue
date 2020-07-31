
using System;
using PayQueue.Definition;
using PayQueue.Impl;

namespace PayQueue.Consuming.Event
{
    internal class ConsumerFactory<S, T> where S: IServiceDefinition, new()
    {
        private Func<IEventConsumer<S, T>> _fact;

        public ConsumerFactory(Func<IEventConsumer<S, T>> fact) => _fact = fact;

        public IEventConsumer<S, T> New() => _fact();
    }
}