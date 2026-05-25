using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sinsam.SingletonSystem
{
    public static class SingletonBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeAutoSingletons()
        {
            var registry = Resources.Load<SingletonRegistry>(SingletonSettings.RegistryResourcePath);

            if (registry == null)
            {
                SingletonLogger.Warning($"[AutoSingleton] Registry not found at Resources/{SingletonSettings.RegistryResourcePath}.");
                return;
            }

            foreach (var prefab in registry.Prefabs)
            {
                if (prefab == null)
                {
                    continue;
                }

                CreateSingletonInstance(prefab);
            }
        }

        private static void CreateSingletonInstance(GameObject prefab)
        {
            var singletonType = GetAutoSingletonType(prefab);

            if (singletonType == null)
            {
                SingletonLogger.Warning($"[AutoSingleton] '{prefab.name}' does not contain a valid AutoSingleton MonoSingleton component.");
                return;
            }

            if (Object.FindFirstObjectByType(singletonType) != null)
            {
                return;
            }

            var instance = Object.Instantiate(prefab);
            instance.name = prefab.name;
            SingletonLogger.Log($"[AutoSingleton] '{prefab.name}' created via SingletonRegistry.");
        }

        private static Type GetAutoSingletonType(GameObject prefab)
        {
            var behaviours = prefab.GetComponents<MonoBehaviour>();

            foreach (var behaviour in behaviours)
            {
                if (behaviour == null)
                {
                    continue;
                }

                var type = behaviour.GetType();

                if (type.GetCustomAttributes(typeof(AutoSingletonAttribute), false).Length > 0 &&
                    IsMonoSingletonType(type))
                {
                    return type;
                }
            }

            return null;
        }

        private static bool IsMonoSingletonType(Type type)
        {
            while (type != null && type != typeof(MonoBehaviour))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(MonoSingleton<>))
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }
    }
}
