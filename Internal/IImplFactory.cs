using System;
using System.Collections.Generic;
using System.Linq;

namespace PayQueue.Internal
{

    internal interface IImplFactory<TImpl>
    {
        TImpl New();
    }


}