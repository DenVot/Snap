namespace Snap.Abstractions
{
    public interface ISpaceProvider<T>
    {
        void WriteData(T obj);
        void ClearData();
        T? GetData();
    }
}