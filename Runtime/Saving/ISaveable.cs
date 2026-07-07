namespace CatAnnaDev.Saving
{
    public interface ISaveable
    {
        string SaveKey { get; }

        object CaptureState();

        void RestoreState(object state);
    }

    public interface ISaveable<TState> : ISaveable
    {
        new TState CaptureState();

        void RestoreState(TState state);
    }
}
