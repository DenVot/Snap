using System;
using System.Threading;
using System.Threading.Tasks;
using Snap.Abstractions;

namespace Snap.Remoting
{
    internal class RemotingStateTracker<T> where T : class
    {
        private readonly RemotingStateTrackerConfiguration<T> _configuration;

        public RemotingStateTracker(RemotingStateTrackerConfiguration<T> configuration)
        {
            _configuration = configuration;
        }

        public Task StartRemoteTracking(CancellationToken? token = null)
        {
            while (!token?.IsCancellationRequested ?? true)
            {
                var cachedDeltas = _configuration.DeltasSpaceProvider.GetData();

                if (cachedDeltas == null)
                {
                    return Task.CompletedTask;
                }

                var result = _configuration.ServerProvider.SendData(cachedDeltas);

                if (!result.Ok)
                {
                    continue;
                }
                
                _configuration.DeltasSpaceProvider.ClearData();
                return Task.CompletedTask;
            }
            
            return Task.CompletedTask;
        }
    }

    internal class RemotingStateTrackerConfiguration<T> where T : class
    {
        public IRemoteServerProvider<T> ServerProvider { get; set; }
        public ISpaceProvider<T> DeltasSpaceProvider { get; set; }
        public TimeSpan Frequency { get; set; }
    }
}