using Snap.Remoting.Types;

namespace Snap.Abstractions
{
    public interface IRemoteServerProvider<T> where T : class
    {
        SnapRemoteResult<T> SendData(T obj);
        SnapRemoteResult<T> GetData();
    }
}