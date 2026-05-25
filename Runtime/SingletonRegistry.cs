using System.Collections.Generic;
using UnityEngine;

namespace Sinsam.SingletonSystem
{
    [CreateAssetMenu(fileName = "SingletonRegistry", menuName = "Singleton/Singleton Registry")]
    public sealed class SingletonRegistry : ScriptableObject
    {
        [SerializeField]
        private List<GameObject> prefabs = new();

        public IReadOnlyList<GameObject> Prefabs => prefabs;

#if UNITY_EDITOR
        internal void SetPrefabs(IEnumerable<GameObject> singletonPrefabs)
        {
            prefabs.Clear();
            prefabs.AddRange(singletonPrefabs);
        }
#endif
    }
}
