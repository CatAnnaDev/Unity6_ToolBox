using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;

namespace CatAnnaDev
{
    public static class CatLog
    {
        private const string RawTag = "[CatAnnaDev]";

        public static void Info(object message, Object context = null)
        {
            Emit(LogType.Log, LogLevel.Info, message, context);
        }

        public static void Warn(object message, Object context = null)
        {
            Emit(LogType.Warning, LogLevel.Warning, message, context);
        }

        public static void Error(object message, Object context = null)
        {
            Emit(LogType.Error, LogLevel.Error, message, context);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("CATANNADEV_VERBOSE")]
        public static void Verbose(object message, Object context = null)
        {
            Emit(LogType.Log, LogLevel.Verbose, message, context);
        }

        public static void InfoFormat(string format, params object[] args)
        {
            Emit(LogType.Log, LogLevel.Info, SafeFormat(format, args), null);
        }

        public static void WarnFormat(string format, params object[] args)
        {
            Emit(LogType.Warning, LogLevel.Warning, SafeFormat(format, args), null);
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            Emit(LogType.Error, LogLevel.Error, SafeFormat(format, args), null);
        }

        public static void Assert(bool condition, object message, Object context = null)
        {
            if (condition)
            {
                return;
            }

            Emit(LogType.Error, LogLevel.Error, message, context);
        }

        private static void Emit(LogType type, LogLevel level, object message, Object context)
        {
            CatAnnaDevSettings settings = CatAnnaDevSettings.Instance;
            if (settings == null)
            {
                LogRaw(type, RawTag + " " + Stringify(message), context);
                return;
            }

            if (!settings.EnableLogging)
            {
                return;
            }

#if !UNITY_EDITOR
            if (!settings.LogInBuilds)
            {
                return;
            }
#endif

            if ((int)level > (int)settings.LogLevel || settings.LogLevel == LogLevel.None)
            {
                return;
            }

            LogRaw(type, BuildTag(settings) + " " + Stringify(message), context);
        }

        private static void LogRaw(LogType type, string text, Object context)
        {
            switch (type)
            {
                case LogType.Warning:
                    Debug.LogWarning(text, context);
                    break;
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    Debug.LogError(text, context);
                    break;
                default:
                    Debug.Log(text, context);
                    break;
            }
        }

        private static string BuildTag(CatAnnaDevSettings settings)
        {
#if UNITY_EDITOR
            string hex = string.IsNullOrEmpty(settings.LogColorHex) ? "#7FD1FF" : settings.LogColorHex;
            return "<color=" + hex + ">" + RawTag + "</color>";
#else
            return RawTag;
#endif
        }

        private static string Stringify(object message)
        {
            return message == null ? "null" : message.ToString();
        }

        private static string SafeFormat(string format, object[] args)
        {
            if (string.IsNullOrEmpty(format))
            {
                return string.Empty;
            }

            if (args == null || args.Length == 0)
            {
                return format;
            }

            try
            {
                return string.Format(format, args);
            }
            catch (System.FormatException)
            {
                return format;
            }
        }
    }
}
