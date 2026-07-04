using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sinsam.SingletonSystem
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;
        private static bool _applicationIsQuitting;
        private static bool _sceneChangeSubscribed;

        public static bool HasInstance => _instance != null;

        public static T Instance
        {
            get
            {
                EnsureSceneChangeSubscribed();

                if (_applicationIsQuitting)
                {
                    SingletonLogger.Warning($"[MonoSingleton] {typeof(T).Name} was requested while the application is quitting.");
                    return null;
                }

                if (!SingletonScenePolicy.IsSceneAllowed(typeof(T)))
                {
                    var scene = SceneManager.GetActiveScene();
                    SingletonLogger.Warning($"[MonoSingleton] {typeof(T).Name} is not registered for scene '{scene.name}'. Instance creation denied.");
                    return null;
                }

                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        var prefab = SingletonScenePolicy.GetPrefab(typeof(T));

                        if (prefab != null)
                        {
                            var instanceObject = Instantiate(prefab);
                            instanceObject.name = prefab.name;
                            _instance = instanceObject.GetComponent<T>();
                        }
                        else
                        {
                            var singletonObject = new GameObject($"{typeof(T).Name} (Singleton)");
                            _instance = singletonObject.AddComponent<T>();
                        }
                    }
                }

                return _instance;
            }
        }

        public static void DestroyInstance()
        {
            if (_instance == null)
            {
                return;
            }

            var instanceObject = _instance.gameObject;
            _instance = null;

            if (Application.isPlaying)
            {
                Destroy(instanceObject);
            }
            else
            {
                DestroyImmediate(instanceObject);
            }
        }

        protected virtual void InitializeSingleton() { }

        protected virtual bool ShouldPersist() => true;

        protected virtual void Awake()
        {
            EnsureSceneChangeSubscribed();

            if (_applicationIsQuitting)
            {
                Destroy(gameObject);
                return;
            }

            var scene = SceneManager.GetActiveScene();

            if (!SingletonScenePolicy.IsSceneAllowed(typeof(T), scene))
            {
                SingletonLogger.Warning($"[MonoSingleton] {typeof(T).Name} is not registered for scene '{scene.name}'. Destroying object.");
                Destroy(gameObject);
                return;
            }

            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = (T)this;
            InitializeSingleton();

            if (ShouldPersist())
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private static void EnsureSceneChangeSubscribed()
        {
            if (_sceneChangeSubscribed)
            {
                return;
            }

            SceneManager.activeSceneChanged -= HandleActiveSceneChanged;
            SceneManager.activeSceneChanged += HandleActiveSceneChanged;
            _sceneChangeSubscribed = true;
        }

        private static void HandleActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            if (_applicationIsQuitting || _instance == null)
            {
                return;
            }

            if (SingletonScenePolicy.IsSceneAllowed(typeof(T), newScene))
            {
                return;
            }

            if (!SingletonScenePolicy.ShouldDestroyWhenSceneNotAllowed(typeof(T)))
            {
                return;
            }

            SingletonLogger.Warning($"[MonoSingleton] {typeof(T).Name} is not registered for scene '{newScene.name}'. DestroyInstance() called.");
            DestroyInstance();
        }
    }
}
