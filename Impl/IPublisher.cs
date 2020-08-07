using System;
using System.Threading.Tasks;
using PayQueue.Definition;
using PayQueue.Impl;

namespace PayQueue.Impl
{
    public interface IPublisher
    {
        Task<PublishResult> Command<S, T>(T message) where S : IServiceDefinition, new();
        Task<PublishResult> Command<S, T>(T message, Action<ICallParameters> parameters) where S : IServiceDefinition, new();
        Task<PublishResult> Publish<T>(T message);
        Task<PublishResult> Publish<T>(T message, Action<ICallParameters> parameters);
    }
}