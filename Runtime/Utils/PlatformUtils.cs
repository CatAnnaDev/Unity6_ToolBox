using UnityEngine;

namespace CatAnnaDev.Utils
{
    public static class PlatformUtils
    {
        public static bool IsEditor
        {
            get { return Application.isEditor; }
        }

        public static bool IsMobile
        {
            get
            {
#if UNITY_IOS || UNITY_ANDROID
                return true;
#else
                return Application.isMobilePlatform;
#endif
            }
        }

        public static bool IsStandalone
        {
            get
            {
#if UNITY_STANDALONE
                return true;
#else
                return false;
#endif
            }
        }

        public static bool IsConsole
        {
            get
            {
#if UNITY_PS4 || UNITY_PS5 || UNITY_XBOXONE || UNITY_GAMECORE || UNITY_SWITCH
                return true;
#else
                return false;
#endif
            }
        }

        public static bool IsWebGL
        {
            get
            {
#if UNITY_WEBGL
                return true;
#else
                return false;
#endif
            }
        }

        public static bool IsDevelopmentBuild
        {
            get { return Debug.isDebugBuild; }
        }

        public static bool HasTouchScreen
        {
            get { return UnityEngine.Input.touchSupported; }
        }

        public static bool IsApplePlatform
        {
            get
            {
#if UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_TVOS
                return true;
#else
                return false;
#endif
            }
        }

        public static RuntimePlatform Platform
        {
            get { return Application.platform; }
        }
    }
}
