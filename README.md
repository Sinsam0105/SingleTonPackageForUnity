# Singleton System

A lightweight Unity package for runtime singletons and attribute-driven auto singleton generation.

## Requirements

- Unity 2022.3 or newer

## Runtime MonoSingleton

Create a singleton component by inheriting from `MonoSingleton<T>`.

```csharp
using Sinsam.SingletonSystem;

[AutoSingleton]
public sealed class GameManager : MonoSingleton<GameManager>
{
    protected override void InitializeSingleton()
    {
        // Initialize once here.
    }
}
```

Access it with:

```csharp
GameManager.Instance;
```

## AutoSingleton generation

Add `[AutoSingleton]` to a concrete `MonoSingleton<T>` class.

```csharp
[AutoSingleton(loadOnStart: true, createPrefab: true)]
public sealed class AudioManager : MonoSingleton<AudioManager>
{
}
```

The editor generator scans attributed singleton types, creates prefabs under:

```text
Assets/Resources/AutoSingletons
```

and writes the startup registry to:

```text
Assets/Resources/SingletonRegistry.asset
```

The generator runs after script reload and can also be run manually from:

```text
Tools > Singleton System > Generate Auto Singleton Registry
```

## Attribute options

- `loadOnStart`: if true, the generated or discovered prefab is added to `SingletonRegistry` and instantiated before the first scene loads.
- `createPrefab`: if true, the editor creates or updates a prefab automatically. If false, the generator searches for an existing prefab under `Assets/Resources/AutoSingletons`.

## Plain C# Singleton

For non-`MonoBehaviour` classes:

```csharp
using Sinsam.SingletonSystem;

public sealed class SaveService : Singleton<SaveService>
{
    public SaveService()
    {
    }
}
```

Access it with:

```csharp
SaveService.Instance;
```
