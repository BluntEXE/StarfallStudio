using StarfallStudio.API.Enums;
using StarfallStudio.API.Helpers;
using StarfallStudio.API.Interface;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin;

namespace StarfallStudio.API;

public class SpawnActor(IDalamudPluginInterface pi) : FuncSubscriber<SpawnFlags, bool, bool, IGameObject?>(pi, Label)
{
    /// <summary> The label. </summary>
    public const string Label = $"StarfallStudio.{nameof(SpawnActor)}.V3";

    /// <inheritdoc cref="IActor.Spawn"/>
    public IGameObject? Invoke()
        => base.Invoke(SpawnFlags.Default, false, false);

    /// <inheritdoc cref="IActor.Spawn"/>
    public IGameObject? Invoke(bool spawnFrozen)
        => base.Invoke(SpawnFlags.Default, false, spawnFrozen);

    /// <inheritdoc cref="IActor.Spawn"/>
    public IGameObject? Invoke(SpawnFlags spawnFlags, bool spawnFrozen)
        => base.Invoke(spawnFlags, false, spawnFrozen);

    /// <inheritdoc cref="IActor.Spawn"/>
    public IGameObject? Invoke(SpawnFlags spawnFlags, bool selectInHierarchy, bool spawnFrozen)
        => base.Invoke(spawnFlags, selectInHierarchy, spawnFrozen);

    public static FuncProvider<SpawnFlags, bool, bool, IGameObject?> Provider(IDalamudPluginInterface pi, IActor api)
        => new(pi, Label, api.Spawn);
}

public class DespawnActor(IDalamudPluginInterface pi) : FuncSubscriber<IGameObject, bool>(pi, Label)
{
    /// <summary> The label. </summary>
    public const string Label = $"StarfallStudio.{nameof(DespawnActor)}.V3";

    /// <inheritdoc cref="IActor.Despawn"/>
    public new bool Invoke(IGameObject gameObject)
        => base.Invoke(gameObject);

    public static FuncProvider<IGameObject, bool> Provider(IDalamudPluginInterface pi, IActor api)
        => new(pi, Label, api.Despawn);
}

public class ActorExists(IDalamudPluginInterface pi) : FuncSubscriber<IGameObject, bool>(pi, Label)
{
    /// <summary> The label. </summary>
    public const string Label = $"StarfallStudio.{nameof(ActorExists)}.V3";

    /// <inheritdoc cref="IActor.Exists"/>
    public new bool Invoke(IGameObject gameObject)
        => base.Invoke(gameObject);

    public static FuncProvider<IGameObject, bool> Provider(IDalamudPluginInterface pi, IActor api)
        => new(pi, Label, api.Exists);
}

public class GetAllActors(IDalamudPluginInterface pi) : FuncSubscriber<IGameObject[]?>(pi, Label)
{
    /// <summary> The label. </summary>
    public const string Label = $"StarfallStudio.{nameof(GetAllActors)}.V3";

    /// <inheritdoc cref="IActor.Exists"/>
    public new IGameObject[]? Invoke()
        => base.Invoke();

    public static FuncProvider<IGameObject[]?> Provider(IDalamudPluginInterface pi, IActor api)
        => new(pi, Label, api.GetAllActors);
}

public class LoadMCDF(IDalamudPluginInterface pi) : FuncSubscriber<IGameObject, string, StarfallStudioApiResult>(pi, Label)
{
    /// <summary> The label. </summary>
    public const string Label = $"StarfallStudio.{nameof(LoadMCDF)}.V3";

    /// <inheritdoc cref="IActor.LoadMCDF"/>
    public new StarfallStudioApiResult Invoke(IGameObject gameObject, string path)
        => base.Invoke(gameObject, path);

    public static FuncProvider<IGameObject, string, StarfallStudioApiResult> Provider(IDalamudPluginInterface pi, IActor api)
        => new(pi, Label, api.LoadMCDF);
}

public class SaveMCDF(IDalamudPluginInterface pi) : FuncSubscriber<IGameObject, string, string, StarfallStudioApiResult>(pi, Label)
{
    /// <summary> The label. </summary>
    public const string Label = $"StarfallStudio.{nameof(SaveMCDF)}.V3";

    /// <inheritdoc cref="IActor.SaveMCDF"/>
    public new StarfallStudioApiResult Invoke(IGameObject gameObject, string path, string description)
        => base.Invoke(gameObject, path, description);

    public static FuncProvider<IGameObject, string, string, StarfallStudioApiResult> Provider(IDalamudPluginInterface pi, IActor api)
        => new(pi, Label, api.SaveMCDF);
}

/// <summary>Invoked when a StarfallStudio Actor is initialized and ready.</summary>
public static class ActorSpawned
{
    /// <summary> The label. </summary>
    public const string Label = $"StarfallStudio.{nameof(ActorSpawned)}";

    /// <summary> Create a new event subscriber. </summary>
    public static StarfallStudioEventSubscriber<IGameObject> Subscriber(IDalamudPluginInterface pi, params Action<IGameObject>[] actions)
        => new(pi, Label, actions);

    /// <summary> Create a provider. </summary>
    public static StarfallStudioEventProvider<IGameObject> Provider(IDalamudPluginInterface pi)
        => new(pi, Label);
}

/// <summary>Invoked when a StarfallStudio Actor is disposed and unavailable.</summary>
public static class ActorDestroyed
{
    /// <summary> The label. </summary>
    public const string Label = $"StarfallStudio.{nameof(ActorDestroyed)}";

    /// <summary> Create a new event subscriber. </summary>
    public static StarfallStudioEventSubscriber<IGameObject> Subscriber(IDalamudPluginInterface pi, params Action<IGameObject>[] actions)
        => new(pi, Label, actions);

    /// <summary> Create a provider. </summary>
    public static StarfallStudioEventProvider<IGameObject> Provider(IDalamudPluginInterface pi)
        => new(pi, Label);
}
