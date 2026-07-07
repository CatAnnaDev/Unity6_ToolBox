namespace CatAnnaDev.Pooling
{
    public interface IObjectPool<T> where T : class
    {
        int CountActive { get; }
        int CountInactive { get; }
        int CountAll { get; }

        T Get();
        void Release(T element);
        void Clear();
        void Prewarm(int count);
    }
}
