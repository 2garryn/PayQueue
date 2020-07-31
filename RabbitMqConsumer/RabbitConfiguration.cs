using System;
using System.Collections.Generic;
using System.Linq;

namespace PayQueue.RabbitMqConsumer
{

    public class RabbitConfiguration
    {

        public string Username { get; set; }
        public string Password { get; set; }
        public string Hostname { get; set; }
        public string VHost { get; set; }
        public string ErrorQueue { get; set; }
        public string ErrorExchange { get; set; }
        public int PublishPoolSize { get; set; }
        public string ServiceLabel {get;set;}


    }


}