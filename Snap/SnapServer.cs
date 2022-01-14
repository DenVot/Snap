using System;
using Snap.Remoting;

namespace Snap
{
    public class SnapServer<T> where T : class
    {
        private readonly SnapConfiguration<T> _config;
        private readonly SnapServerExceptionsThrower<T> _exceptionsThrower;

        public SnapServer(SnapConfiguration<T> config)
        {
            _config = config;
            _exceptionsThrower = new SnapServerExceptionsThrower<T>(config);
        }

        public T? GetData()
        {
            _exceptionsThrower.ThrowIfSyncServerNull();

            var snapResult = _config.RemoteServerProvider!.GetData();

            return snapResult.Ok ? snapResult.Object! : _config.CacheSpaceProvider.GetData();
        }

        public void SendData(T obj)
        {
            _exceptionsThrower.ThrowIfSyncServerNull();

            var snapResult = _config.RemoteServerProvider!.SendData(obj);

            if (snapResult.Ok) return;
            
            _config.DeltasSpaceProvider.WriteData(obj);
            InitializeTrackerAndStart();
        }

        private void InitializeTrackerAndStart()
        {
            var tracker = new RemotingStateTracker<T>(new RemotingStateTrackerConfiguration<T>()
            {
                DeltasSpaceProvider = _config.DeltasSpaceProvider,
                Frequency = TimeSpan.FromMinutes(10),
                ServerProvider = _config.RemoteServerProvider
            });

            tracker.StartRemoteTracking();
        }
    }
}