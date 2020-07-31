using System;
using System.Threading.Tasks;
using PayQueue.Impl;

namespace PayQueue.Internal
{
    internal class DefaultMiddleware: IMiddlewareEvent, IMiddlewareCommand
    {
        public async Task Invoke<T>(MessageContext<T> context, Func<MessageContext<T>, Task> next) => await next(context);
    }
}