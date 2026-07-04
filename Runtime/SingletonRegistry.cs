using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sinsam.SingletonSystem
{
    [CreateAssetMenu(fileName = "SingletonRegistry", menuName = "Singleton/Singleton Registry")]
    public sealed class SingletonRegistry : ScriptableObject
    {
        [SerializeField]
        private List<GameObject> prefabs = new();

        [SerializeField]
        private List<SingletonSceneRule> sceneRules = new();

        public IReadOnlyList<GameObject> Prefabs => prefabs;
        public IReadOnlyList<SingletonSceneRule> SceneRules => sceneRules;

        public bool IsSceneAllowed(Type singletonType, Scene scene)
        {
            var rule = FindSceneRule(singletonType);
            return rule == null || rule.IsSceneAllowed(scene);
        }

        public bool ShouldDestroyWhenSceneNotAllowed(Type singletonType)
        {
            var rule = FindSceneRule(singletonType);
            return rule != null && rule.DestroyWhenSceneNotAllowed;
        }

        public GameObject GetPrefab(Type singletonType)
        {
            var rule = FindSceneRule(singletonType);

            if (rule?.Prefab != null)
            {
                return rule.Prefab;
            }

            foreach (var prefab in prefabs)
            {
                if (prefab != null && prefab.GetComponent(singletonType) != null)
                {
                    return prefab;
                }
            }

            return null;
        }

        private SingletonSceneRule FindSceneRule(Type singletonType)
        {
            foreach (var rule in sceneRules)
            {
                if (rule != null && rule.Matches(singletonType))
                {
                    return rule;
                }
            }

            return null;
        }

#if UNITY_EDITOR
        public void SetPrefabs(IEnumerable<GameObject> singletonPrefabs)
        {
            prefabs.Clear();
            prefabs.AddRange(singletonPrefabs);
        }
#endif
    }

    [Serializable]
    public sealed class SingletonSceneRule
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private bool allowAllScenes = true;

        [SerializeField]
        private bool destroyWhenSceneNotAllowed = true;

        [SerializeField]
        private List<string> sceneNames = new();

        public GameObject Prefab => prefab;
        public bool AllowAllScenes => allowAllScenes;
        public bool DestroyWhenSceneNotAllowed => destroyWhenSceneNotAllowed;
        public IReadOnlyList<string> SceneNames => sceneNames;

        public bool Matches(Type singletonType)
        {
            return prefab != null && prefab.GetComponent(singletonType) != null;
        }

        public bool IsSceneAllowed(Scene scene)
        {
            if (allowAllScenes)
            {
                return true;
            }

            foreach (var sceneName in sceneNames)
            {
                if (string.IsNullOrWhiteSpace(sceneName))
                {
                    continue;
                }

                if (string.Equals(sceneName, scene.name, StringComparison.Ordinal) ||
                    string.Equals(sceneName, scene.path, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
