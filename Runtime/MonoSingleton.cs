using UnityEngine;

namespace Sinsam.SingletonSystem
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;
        private static bool _applicationIsQuitting;

        public static bool HasInstance => _instance != null;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    SingletonLogger.Warning($"[MonoSingleton] {typeof(T).Name} was requested while the application is quitting.");
                    return null;
                }

                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        var singletonObject = new GameObject($"{typeof(T).Name} (Singleton)");
                        _instance = singletonObject.AddComponent<T>();
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
            if (_applicationIsQuitting)
            {
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
    }
}
