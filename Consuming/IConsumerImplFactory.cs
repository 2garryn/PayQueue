using System;
using System.Collections.Generic;
using System.Text;
using PayQueue.Definition;

namespace PayQueue.Consuming
{
    internal interface IConsumerImplFactory
    {

        Event.ConsumerFactory<TServDef, T> NewEventConsumerFactory<TServDef, T>()
            where TServDef : IServiceDefinition, new();

        Command.ConsumerFactory<T> NewCommandConsumerFactory<T>();
    }
}
