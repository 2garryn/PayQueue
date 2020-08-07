using System;
using System.Collections.Generic;
using System.Linq;

namespace PayQueue.Impl
{
    public enum PublishStatus
    {
        Published,
        NoDestination
    }

    public class PublishResult
    {
        public PublishStatus PublishStatus {get; internal set;}
    }


}