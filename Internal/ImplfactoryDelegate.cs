using System;

namespace PayQueue.Internal
{
    internal class ImplfactoryDelegate<TImpl>: IImplFactory<TImpl>
    {
        private Func<TImpl> _impl;
        public ImplfactoryDelegate(Func<TImpl> impl) => _impl = impl;
        public TImpl New() => _impl();
    }
}