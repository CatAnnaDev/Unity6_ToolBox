using UnityEngine;

namespace CatAnnaDev
{
    public enum LogLevel
    {
        None = 0,
        Error = 1,
        Warning = 2,
        Info = 3,
        Verbose = 4
    }

    [CreateAssetMenu(menuName = "CatAnnaDev/Settings", fileName = "CatAnnaDevSettings")]
    public sealed class CatAnnaDevSettings : ScriptableObject
    {
        public const string ResourcePath = "CatAnnaDevSettings";

        [SerializeField] private bool enableLogging = true;
        [SerializeField] private LogLevel logLevel = LogLevel.Info;
        [SerializeField] private string logColorHex = "#7FD1FF";
        [SerializeField] private bool logInBuilds = false;
        [SerializeField] private int defaultPoolCapacity = 16;
        [SerializeField] private int defaultPoolMaxSize = 1024;

        public bool EnableLogging => enableLogging;
        public LogLevel LogLevel => logLevel;
        public string LogColorHex => logColorHex;
        public bool LogInBuilds => logInBuilds;
        public int DefaultPoolCapacity => defaultPoolCapacity;
        public int DefaultPoolMaxSize => defaultPoolMaxSize;

        private static CatAnnaDevSettings cached;

        public static CatAnnaDevSettings Instance
        {
            get
            {
                if (cached != null)
                {
                    return cached;
                }

                cached = Resources.Load<CatAnnaDevSettings>(ResourcePath);
                if (cached == null)
                {
                    cached = CreateDefault();
                }

                return cached;
            }
        }

        private static CatAnnaDevSettings CreateDefault()
        {
            CatAnnaDevSettings instance = CreateInstance<CatAnnaDevSettings>();
            instance.name = ResourcePath;
            instance.hideFlags = HideFlags.HideAndDontSave;
            return instance;
        }
    }
}
