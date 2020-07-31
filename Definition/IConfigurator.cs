using System;

namespace PayQueue.Definition 
{
    public interface IConfigurator
    {
        void ConsumeCommand<T>();
        void ConsumeEvent<S, T>() where S: IServiceDefinition, new();
        void ConsumeEvent<S, T>(string key) where S: IServiceDefinition, new();
        void Command<S, T>() where S: IServiceDefinition, new();
        void PublishEvent<T>();
        void PublishEvent<T>(Func<T, string> routeFormatter);
    }

}