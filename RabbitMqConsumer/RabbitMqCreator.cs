using System;
using System.Collections.Generic;
using System.Linq;
using PayQueue.Definition;
using PayQueue.Impl;

namespace PayQueue.RabbitMqConsumer
{

    public static class RabbitMqCreator
    {
        
        public static PayQueueCreator<TServDef, TImpl> New<TServDef, TImpl>(Action<RabbitConfiguration> confAct)         
            where TServDef : IServiceDefinition, new()
            where TImpl : IServiceImpl<TServDef>
        {
            var c = new RabbitConfiguration();
            confAct(c);
            var consumer = new RabbitMqConsumer(c);
            return new PayQueueCreator<TServDef, TImpl>(consumer);
        }

    }


}