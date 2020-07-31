using System;

namespace PayQueue.Impl
{

    public class MessageContext<T>
    {
        public T Message { get; internal set; }
        public Guid RequestId { get; internal set; }
        public string SourceService { get; internal set; }
        public string SourceHost { get; internal set; }
        public DateTime PublishTimestamp { get; internal set; }
        public Guid? ConversationId { get; internal set; }
        public IPublisher Publisher {get; internal set;}
    }


}