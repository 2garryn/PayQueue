using System;

namespace PayQueue.Exceptions
{
    public class PayQueueException: Exception
    {
        public PayQueueException() {}

        public PayQueueException(string message) : base(message) {}

        public PayQueueException(string message, Exception inner) : base(message, inner) {}
    }
}