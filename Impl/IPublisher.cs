using System;
using System.Threading.Tasks;
using PayQueue.Definition;


namespace PayQueue.Impl
{
    public interface IPublisher
    {
        Task Command<S, T>(T message) where S : IServiceDefinition, new();
        Task Command<S, T>(T message, Action<ICallParameters> parameters) where S : IServiceDefinition, new();
        Task Publish<T>(T message);
        Task Publish<T>(T message, Action<ICallParameters> parameters);
    }
}