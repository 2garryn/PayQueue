using System;
using PayQueue.Impl;

namespace PayQueue.Consuming.Command
{
    internal class ConsumerFactory<T>
    {

        private Func<ICommandConsumer<T>> _fact;

        public ConsumerFactory(Func<ICommandConsumer<T>> fact) => _fact = fact;
        public ICommandConsumer<T> New() => _fact();
    }
}