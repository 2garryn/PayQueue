using System;

namespace PayQueue.Internal
{
    internal  class Routes
    {
        private string _prefix;
        private string _consumerLabel;

        public Routes(string prefix, string consumerLabel) =>
            (_prefix, _consumerLabel) = (prefix, consumerLabel);

        public string ConsumeCommandQueue() => 
            _prefix + ":" + _consumerLabel + ":commands";
        
        public string PublishCommandExchange(string label) => 
            _prefix + ":" + label + ":commands";
        public string ConsumeEventQueue() => 
            _prefix + ":" + _consumerLabel + ":events";
        public string ConsumeEventExchange(string label, Type t, string routeKey = null) =>
            _prefix + ":" + label + ":events" + ":" + t.Name + (routeKey == null ? "" : ":" + routeKey);
        

        public Func<object, string>  PublishEventExchange<T>(Func<T, string> route = null)
        {
            var tName = typeof(T).Name;
            if (route == null)
            {
                return (obj) => _prefix + ":" + _consumerLabel + ":events:" + tName;
            }
            return (obj) => _prefix + ":" + _consumerLabel + ":events" + ":" + tName + ":" + route((T) obj);
        }
        


    }
}