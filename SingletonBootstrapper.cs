using UnityEngine;

public static class SingletonBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeAutoSingletons()
    {
        var registry = Resources.Load<SingletonRegistry>(Constant.Singleton.REGISTRY_RESOURCE_PATH);

        if (registry == null)
        {
            if (Constant.Singleton.SHOW_SYSTEM_LOGS)
            {
                D.LogError($"[AutoSingleton] Registry not found at Resources/{Constant.Singleton.REGISTRY_RESOURCE_PATH}");
            }
            return;
        }

        foreach (var prefab in registry.prefabs)
        {
            if (prefab == null) continue;
            CreateSingletonInstance(prefab);
        }
    }

    private static void CreateSingletonInstance(GameObject prefab)
    {
        if (prefab.TryGetComponent<MonoBehaviour>(out var component))
        {
            var type = component.GetType();

            if (Object.FindFirstObjectByType(type) == null)
            {
                GameObject instance = Object.Instantiate(prefab);
                instance.name = prefab.name;
                Object.DontDestroyOnLoad(instance);

                if (Constant.Singleton.SHOW_SYSTEM_LOGS)
                {
                    D.Log($"[AutoSingleton] '{prefab.name}' created via Registry.");
                }
            }
        }
    }
}