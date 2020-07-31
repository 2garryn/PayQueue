using System;
using System.Collections.Generic;
using System.Linq;

namespace PayQueue.RabbitMqConsumer
{

    public class PayQueueError
    {
        public string ServiceLabel {get;set;}
        public string ConsumeMessageType {get;set;}
        public string ConsumeParams {get;set;}
        public string ExceptionType {get;set;}
        public string ExceptionMessage {get;set;}
        public string ExceptionStacktrace {get;set;}
        public string Exchange {get;set;}

    }


}