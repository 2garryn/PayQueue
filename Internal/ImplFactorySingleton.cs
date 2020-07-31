using System;

namespace PayQueue.Internal
{
    internal class ImplfactorySingleton<TImpl>: IImplFactory<TImpl>
    {
        private TImpl _impl;
        public ImplfactorySingleton(TImpl impl) => _impl = impl;
        public TImpl New() => _impl;
    }
}