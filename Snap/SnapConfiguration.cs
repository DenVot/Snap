using System;
using Snap.Abstractions;

namespace Snap
{
    public struct SnapConfiguration<T> where T : class
    {
        public ISpaceProvider<T> DeltasSpaceProvider { get; set; }
        public ISpaceProvider<T> CacheSpaceProvider { get; set; }
        public IRemoteServerProvider<T> RemoteServerProvider { get; set; }

        public Action<Action>? CustomStartup { get; set; }

        public SnapServer<T> ConfigureServer()
        {
            return new SnapServer<T>(this);
        }
    }
}