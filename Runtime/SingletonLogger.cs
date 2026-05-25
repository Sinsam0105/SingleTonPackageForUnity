using System.Diagnostics;

namespace Sinsam.SingletonSystem
{
    internal static class SingletonLogger
    {
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message) => UnityEngine.Debug.Log(message);

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Warning(object message) => UnityEngine.Debug.LogWarning(message);

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Error(object message) => UnityEngine.Debug.LogError(message);
    }
}
