using System.Diagnostics;
using UnityEngine;

namespace Sinsam.SingletonSystem
{
    internal static class SingletonLogger
    {
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message) => Debug.Log(message);

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Warning(object message) => Debug.LogWarning(message);

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Error(object message) => Debug.LogError(message);
    }
}
