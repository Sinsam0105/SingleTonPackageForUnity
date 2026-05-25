using System;

namespace Sinsam.SingletonSystem
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AutoSingletonAttribute : Attribute
    {
        public bool LoadOnStart { get; }
        public bool CreatePrefab { get; }

        public AutoSingletonAttribute(bool loadOnStart = true, bool createPrefab = true)
        {
            LoadOnStart = loadOnStart;
            CreatePrefab = createPrefab;
        }
    }
}
