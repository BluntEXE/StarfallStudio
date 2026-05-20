using StarfallStudio.API.Helpers;
using StarfallStudio.API.Interface;
using Dalamud.Plugin;

namespace StarfallStudio.API;

public class FreezePhysics(IDalamudPluginInterface pi) : FuncSubscriber<bool>(pi, Label)
{
    /// <summary> The label. </summary>
    public const string Label = $"StarfallStudio.{nameof(FreezePhysics)}.V3";

    /// <inheritdoc cref="IEnvironment.FreezePhysics"/>
    public new bool Invoke()
        => base.Invoke();

    public static FuncProvider<bool> Provider(IDalamudPluginInterface pi, IEnvironment api)
        => new(pi, Label, api.FreezePhysics);
}

public class UnFreezePhysics(IDalamudPluginInterface pi) : FuncSubscriber<bool>(pi, Label)
{
    /// <summary> The label. </summary>
    public const string Label = $"StarfallStudio.{nameof(UnFreezePhysics)}.V3";

    /// <inheritdoc cref="IEnvironment.UnFreezePhysics"/>
    public new bool Invoke()
        => base.Invoke();

    public static FuncProvider<bool> Provider(IDalamudPluginInterface pi, IEnvironment api)
        => new(pi, Label, api.UnFreezePhysics);
}
