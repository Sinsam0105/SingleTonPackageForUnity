using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Sinsam.SingletonSystem
{
    public static class SingletonBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeAutoSingletons()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;

            CreateSingletonsForScene(SceneManager.GetActiveScene());
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CreateSingletonsForScene(scene);
        }

        private static void CreateSingletonsForScene(Scene scene)
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

                var singletonType = GetAutoSingletonType(prefab);

                if (singletonType == null)
                {
                    SingletonLogger.Warning($"[AutoSingleton] '{prefab.name}' does not contain a valid AutoSingleton MonoSingleton component.");
                    continue;
                }

                if (!registry.IsSceneAllowed(singletonType, scene))
                {
                    continue;
                }

                CreateSingletonInstance(prefab, singletonType);
            }
        }

        private static void CreateSingletonInstance(GameObject prefab, Type singletonType)
        {
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
