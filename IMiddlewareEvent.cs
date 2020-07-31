using System;
using System.Threading.Tasks;
using PayQueue.Impl;

namespace PayQueue
{
    public interface IMiddlewareEvent
    {
        Task Invoke<TEvent>(MessageContext<TEvent> messageContext, Func<MessageContext<TEvent>, Task> next);
    }
}