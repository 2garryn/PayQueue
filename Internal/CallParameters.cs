using System;
using System.Collections.Generic;
using System.Linq;
using PayQueue.Impl;

namespace PayQueue.Internal
{

    internal class CallParameters: ICallParameters
    {
        public Guid? ConversationId {set; get;}
        public Guid? RequestId { get; set; }
    }


}