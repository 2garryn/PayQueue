using System;
using System.Threading.Tasks;
using PayQueue.Impl;


namespace PayQueue
{
    public interface IMiddlewareCommand
    {
        Task Invoke<TCommand>(MessageContext<TCommand> messageContext, Func<MessageContext<TCommand>, Task> next);
    }
}