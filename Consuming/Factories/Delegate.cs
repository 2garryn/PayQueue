using System;
using System.Collections.Generic;
using System.Text;
using PayQueue.Definition;
using PayQueue.Impl;
using PayQueue.Consuming;

namespace PayQueue.Consuming.Factories
{

    class Delegate<TImpl> : IConsumerImplFactory
    {
        
        private Func<TImpl> _d;
        public Delegate(Func<TImpl> d) => _d = d;

        public Event.ConsumerFactory<TServDef, T> NewEventConsumerFactory<TServDef, T>()
            where TServDef : IServiceDefinition, new() => 
                new Event.ConsumerFactory<TServDef, T>(() => (IEventConsumer<TServDef, T>) _d());
        
        public Command.ConsumerFactory<T> NewCommandConsumerFactory<T>() => 
            new Command.ConsumerFactory<T>(() => (ICommandConsumer<T>) _d());

            
    }
}