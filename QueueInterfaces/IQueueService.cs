using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayQueue.Impl;

namespace PayQueue.QueueInterfaces
{
    internal interface IQueueConsumer
    {
        void RegisterCommandConsumer(string queue, IConsumeExecutor executor);
        void RegisterEventConsumer(string queue, string[] dispatch, IConsumeExecutor executor); 
        IExchangePublisher GetExchangePublisher();
        Task Stop();
    }

    internal interface IExchangePublisher
    {
        Task<PublishResult> PublishEvent(string endpoint, string messageType, byte[] data);
        Task<PublishResult> Command(string endpoint, string messageType, byte[] data);
    }

    internal interface IConsumeExecutor
    {
        Task Execute(IExchangePublisher exchangePublisher, Func<string> messageType, byte[] data, ConsumeMessageMetadata messageMetadata);
    }


    internal class  ConsumeMessageMetadata
    {
        public string Queue {get;set;}
        public string Exchange {get;set;}
    }
    
}