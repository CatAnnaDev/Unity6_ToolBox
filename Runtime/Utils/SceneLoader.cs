using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using CatAnnaDev.Services;

namespace CatAnnaDev.Utils
{
    public sealed class SceneLoader : PersistentSingleton<SceneLoader>
    {
        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
        }

        public event Action<float> ProgressChanged;
        public event Action<string> SceneLoadStarted;
        public event Action<string> SceneLoadCompleted;

        public static void Load(string sceneName, Action onComplete = null, Action<float> onProgress = null)
        {
            Instance.LoadInternal(sceneName, LoadSceneMode.Single, onComplete, onProgress, true);
        }

        public static void LoadAdditive(string sceneName, Action onComplete = null, Action<float> onProgress = null)
        {
            Instance.LoadInternal(sceneName, LoadSceneMode.Additive, onComplete, onProgress, true);
        }

        public static void Unload(string sceneName, Action onComplete = null)
        {
            Instance.UnloadInternal(sceneName, onComplete);
        }

        public static void Reload(Action onComplete = null, Action<float> onProgress = null)
        {
            string current = SceneManager.GetActiveScene().name;
            Instance.LoadInternal(current, LoadSceneMode.Single, onComplete, onProgress, true);
        }

        private void LoadInternal(string sceneName, LoadSceneMode mode, Action onComplete, Action<float> onProgress, bool allowActivation)
        {
            if (isBusy)
            {
                CatLog.Warn(string.Concat("SceneLoader is busy, ignoring load request for ", sceneName));
                return;
            }
            if (string.IsNullOrEmpty(sceneName))
            {
                CatLog.Error("SceneLoader received an empty scene name.");
                return;
            }
            StartCoroutine(LoadRoutine(sceneName, mode, onComplete, onProgress, allowActivation));
        }

        private IEnumerator LoadRoutine(string sceneName, LoadSceneMode mode, Action onComplete, Action<float> onProgress, bool allowActivation)
        {
            isBusy = true;
            SceneLoadStarted?.Invoke(sceneName);

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, mode);
            if (operation == null)
            {
                CatLog.Error(string.Concat("Failed to start loading scene ", sceneName));
                isBusy = false;
                yield break;
            }

            operation.allowSceneActivation = allowActivation;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                onProgress?.Invoke(progress);
                ProgressChanged?.Invoke(progress);

                if (!allowActivation && operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }
                yield return null;
            }

            onProgress?.Invoke(1f);
            ProgressChanged?.Invoke(1f);
            onComplete?.Invoke();
            SceneLoadCompleted?.Invoke(sceneName);
            isBusy = false;
        }

        private void UnloadInternal(string sceneName, Action onComplete)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid() || !scene.isLoaded)
            {
                CatLog.Warn(string.Concat("SceneLoader cannot unload scene that is not loaded: ", sceneName));
                onComplete?.Invoke();
                return;
            }
            StartCoroutine(UnloadRoutine(sceneName, onComplete));
        }

        private IEnumerator UnloadRoutine(string sceneName, Action onComplete)
        {
            AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);
            if (operation != null)
            {
                while (!operation.isDone)
                {
                    yield return null;
                }
            }
            onComplete?.Invoke();
        }
    }
}
