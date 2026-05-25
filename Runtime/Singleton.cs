namespace Sinsam.SingletonSystem
{
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private static readonly object SyncRoot = new();
        private static T _instance;

        public static bool HasInstance => _instance != null;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                lock (SyncRoot)
                {
                    _instance ??= new T();
                }

                return _instance;
            }
        }

        public static void ResetInstance()
        {
            lock (SyncRoot)
            {
                _instance = null;
            }
        }
    }
}
