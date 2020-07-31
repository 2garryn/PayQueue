using System;
using System.Collections.Generic;
using System.Text;
using PayQueue.Definition;
using PayQueue.Impl;
using PayQueue.Consuming;

namespace PayQueue.Consuming.Factories
{

    class Singleton<TImpl> : IConsumerImplFactory
    {

        private TImpl _singleton;

        public Singleton(TImpl singleton) => _singleton = singleton;

        public Event.ConsumerFactory<TServDef, T> NewEventConsumerFactory<TServDef, T>()
            where TServDef : IServiceDefinition, new()
        {
            var consumer = (IEventConsumer<TServDef, T>) _singleton;
            return new Event.ConsumerFactory<TServDef, T>(() => consumer);
        }

        public Command.ConsumerFactory<T> NewCommandConsumerFactory<T>()
        {
            var consumer = (ICommandConsumer<T>)_singleton;
            return new Command.ConsumerFactory<T>(() => consumer);
        }
    }
}
