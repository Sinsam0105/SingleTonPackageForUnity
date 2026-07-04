using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sinsam.SingletonSystem
{
    internal static class SingletonScenePolicy
    {
        private static SingletonRegistry registry;
        private static bool registryLoaded;

        private static SingletonRegistry Registry
        {
            get
            {
                if (registryLoaded)
                {
                    return registry;
                }

                registry = Resources.Load<SingletonRegistry>(SingletonSettings.RegistryResourcePath);
                registryLoaded = true;
                return registry;
            }
        }

        public static bool IsSceneAllowed(Type singletonType)
        {
            return IsSceneAllowed(singletonType, SceneManager.GetActiveScene());
        }

        public static bool IsSceneAllowed(Type singletonType, Scene scene)
        {
            var singletonRegistry = Registry;
            return singletonRegistry == null || singletonRegistry.IsSceneAllowed(singletonType, scene);
        }

        public static bool ShouldDestroyWhenSceneNotAllowed(Type singletonType)
        {
            var singletonRegistry = Registry;
            return singletonRegistry != null && singletonRegistry.ShouldDestroyWhenSceneNotAllowed(singletonType);
        }

        public static GameObject GetPrefab(Type singletonType)
        {
            return Registry != null ? Registry.GetPrefab(singletonType) : null;
        }
    }
}
