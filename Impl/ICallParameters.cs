using System;

namespace PayQueue.Impl
{
    public interface ICallParameters
    {
        Guid? ConversationId {set; get;}
        Guid? RequestId { set; get; }
    }
}