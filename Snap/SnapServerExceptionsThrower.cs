using System;

namespace Snap
{
    internal class SnapServerExceptionsThrower<T> where T : class
    {
        private readonly SnapConfiguration<T> _configuration;

        public SnapServerExceptionsThrower(SnapConfiguration<T> configuration)
        {
            _configuration = configuration;
        }

        public void ThrowIfSyncServerNull()
        {
            if (_configuration.RemoteServerProvider == null)
            {
                throw new NullReferenceException("Failed to fetch sync server interface instance");
            }
        }
    }
}