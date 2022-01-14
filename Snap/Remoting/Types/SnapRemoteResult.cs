namespace Snap.Remoting.Types
{
    public class SnapRemoteResult<T> where T : class
    {
        public bool Ok { get; set; }
        public T? Object { get; set; }
    }
}