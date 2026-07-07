namespace CatAnnaDev.Scheduling
{
    public readonly struct ScheduledHandle
    {
        readonly ScheduledTask _task;
        readonly int _version;

        internal ScheduledHandle(ScheduledTask task, int version)
        {
            _task = task;
            _version = version;
        }

        public static ScheduledHandle None
        {
            get { return default; }
        }

        public bool IsActive
        {
            get { return _task != null && _task.version == _version && _task.active; }
        }

        public void Cancel()
        {
            if (_task != null && _task.version == _version)
            {
                _task.active = false;
            }
        }
    }
}
