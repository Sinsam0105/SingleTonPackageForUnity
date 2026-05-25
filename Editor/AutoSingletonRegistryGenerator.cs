using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Sinsam.SingletonSystem.Editor
{
    public static class AutoSingletonRegistryGenerator
    {
        private const string MenuPath = "Tools/Singleton System/Generate Auto Singleton Registry";

        [MenuItem(MenuPath)]
        public static void Generate()
        {
            EnsureFolder("Assets", "Resources");
            EnsureFolder("Assets/Resources", "AutoSingletons");

            var registry = LoadOrCreateRegistry();
            var loadOnStartPrefabs = new List<GameObject>();
            var singletonTypes = TypeCache.GetTypesWithAttribute<AutoSingletonAttribute>()
                .Where(IsConcreteMonoSingletonType)
                .OrderBy(type => type.FullName)
                .ToArray();

            foreach (var singletonType in singletonTypes)
            {
                var attribute = (AutoSingletonAttribute)Attribute.GetCustomAttribute(singletonType, typeof(AutoSingletonAttribute));
                GameObject prefab = null;

                if (attribute.CreatePrefab)
                {
                    prefab = CreateOrUpdatePrefab(singletonType);
                }
                else
                {
                    prefab = FindPrefabForType(singletonType);
                }

                if (attribute.LoadOnStart && prefab != null)
                {
                    loadOnStartPrefabs.Add(prefab);
                }
            }

            registry.SetPrefabs(loadOnStartPrefabs);
            EditorUtility.SetDirty(registry);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[AutoSingleton] Generated registry with {loadOnStartPrefabs.Count} startup singleton prefab(s).");
        }

        [InitializeOnLoadMethod]
        private static void GenerateAfterScriptsReload()
        {
            EditorApplication.delayCall += () =>
            {
                if (Application.isPlayingOrWillChangePlaymode)
                {
                    return;
                }

                Generate();
            };
        }

        private static SingletonRegistry LoadOrCreateRegistry()
        {
            var registry = AssetDatabase.LoadAssetAtPath<SingletonRegistry>(SingletonSettings.RegistryAssetPath);

            if (registry != null)
            {
                return registry;
            }

            registry = ScriptableObject.CreateInstance<SingletonRegistry>();
            AssetDatabase.CreateAsset(registry, SingletonSettings.RegistryAssetPath);
            return registry;
        }

        private static GameObject CreateOrUpdatePrefab(Type singletonType)
        {
            var prefabPath = GetPrefabPath(singletonType);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab != null)
            {
                if (prefab.GetComponent(singletonType) == null)
                {
                    var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    instance.AddComponent(singletonType);
                    PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                    UnityEngine.Object.DestroyImmediate(instance);
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                }

                return prefab;
            }

            var gameObject = new GameObject(singletonType.Name);
            gameObject.AddComponent(singletonType);
            PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath);
            UnityEngine.Object.DestroyImmediate(gameObject);

            return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }

        private static GameObject FindPrefabForType(Type singletonType)
        {
            var guids = AssetDatabase.FindAssets("t:Prefab", new[] { SingletonSettings.PrefabSavePath });

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab != null && prefab.GetComponent(singletonType) != null)
                {
                    return prefab;
                }
            }

            Debug.LogWarning($"[AutoSingleton] {singletonType.FullName} has CreatePrefab=false, but no prefab was found under {SingletonSettings.PrefabSavePath}.");
            return null;
        }

        private static string GetPrefabPath(Type singletonType)
        {
            var safeName = singletonType.FullName.Replace('.', '_').Replace('+', '_');
            return Path.Combine(SingletonSettings.PrefabSavePath, $"{safeName}.prefab").Replace('\\', '/');
        }

        private static bool IsConcreteMonoSingletonType(Type type)
        {
            return type is { IsAbstract: false, IsGenericTypeDefinition: false } && IsMonoSingletonType(type);
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

        private static void EnsureFolder(string parent, string child)
        {
            var path = $"{parent}/{child}";

            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }
    }
}
