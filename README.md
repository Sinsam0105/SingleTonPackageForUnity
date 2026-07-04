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

`Instance` automatically creates the singleton when it is missing, unless the singleton is blocked by a scene rule.

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

- `loadOnStart`: if true, the generated or discovered prefab is added to `SingletonRegistry` and automatically instantiated when allowed by the current scene rules.
- `createPrefab`: if true, the editor creates or updates a prefab automatically. If false, the generator searches for an existing prefab under `Assets/Resources/AutoSingletons`.

## Scene scoped singletons

`SingletonRegistry` can restrict a singleton to specific scenes. This is useful for managers such as `BattleManager`, `StageManager`, or `DungeonManager` that should exist only inside a gameplay context.

Open:

```text
Assets/Resources/SingletonRegistry.asset
```

Then add an entry to `Scene Rules`:

```text
Prefab: BattleManager
Allow All Scenes: false
Destroy When Scene Not Allowed: true
Scene Names:
  - BattleScene
  - BossBattleScene
```

With that setup:

- `BattleManager` is created only when the active or loaded scene is `BattleScene` or `BossBattleScene`.
- `BattleManager.Instance` returns `null` and logs a warning outside registered scenes.
- If a persistent `BattleManager` exists and the active scene changes to an unregistered scene, `BattleManager.DestroyInstance()` is called automatically.
- If a `BattleManager` object is placed directly in an unregistered scene, it is destroyed during `Awake()`.

Scene names can be either `scene.name`, such as `BattleScene`, or `scene.path`, such as `Assets/Scenes/BattleScene.unity`.

Rules are optional. If a singleton has no scene rule, it keeps the previous global behavior and can be created in any scene.

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
