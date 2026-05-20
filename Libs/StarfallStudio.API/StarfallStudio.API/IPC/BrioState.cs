using StarfallStudio.API.Helpers;
using StarfallStudio.API.Interface;
using Dalamud.Plugin;

namespace StarfallStudio.API;

/// <inheritdoc cref="IState.ApiVersion"/>
public class ApiVersion(IDalamudPluginInterface pi) : FuncSubscriber<(int Breaking, int Feature)>(pi, Label)
{
    /// <summary> The label. </summary>
    public const string Label = $"StarfallStudio.{nameof(ApiVersion)}";

    /// <inheritdoc cref="IState.ApiVersion"/>
    public new (int Breaking, int Feature) Invoke()
        => base.Invoke();

    /// <summary> Create a provider. </summary>
    public static FuncProvider<(int Breaking, int Feature)> Provider(IDalamudPluginInterface pi, IState api)
        => new(pi, Label, () => api.ApiVersion);
}

/// <inheritdoc cref="IState.IsAvailable"/>
public class IsAvailable(IDalamudPluginInterface pi) : FuncSubscriber<bool>(pi, Label)
{
    /// <summary> The label. </summary>
    public const string Label = $"StarfallStudio.{nameof(IsAvailable)}";

    /// <inheritdoc cref="IState.IsAvailable"/>
    public new bool Invoke()
        => base.Invoke();

    /// <summary> Create a provider. </summary>
    public static FuncProvider<bool> Provider(IDalamudPluginInterface pi, IState api)
        => new(pi, Label, () => api.IsAvailable);
}

/// <inheritdoc cref="IState.IsValidGPoseSession"/>
public class IsValidGPoseSession(IDalamudPluginInterface pi) : FuncSubscriber<bool>(pi, Label)
{
    /// <summary> The label. </summary>
    public const string Label = $"StarfallStudio.{nameof(IsAvailable)}";

    /// <inheritdoc cref="IState.IsValidGPoseSession"/>
    public new bool Invoke()
        => base.Invoke();

    /// <summary> Create a provider. </summary>
    public static FuncProvider<bool> Provider(IDalamudPluginInterface pi, IState api)
        => new(pi, Label, () => api.IsValidGPoseSession);
}

/// <summary>Invoked when StarfallStudio is initialized and ready.</summary>
public static class Initialized
{
    /// <summary> The label. </summary>
    public const string Label = $"StarfallStudio.{nameof(Initialized)}";

    /// <summary> Create a new event subscriber. </summary>
    public static StarfallStudioEventSubscriber Subscriber(IDalamudPluginInterface pi, params Action[] actions)
        => new(pi, Label, actions);

    /// <summary> Create a provider. </summary>
    public static StarfallStudioEventProvider Provider(IDalamudPluginInterface pi)
        => new(pi, Label);
}

/// <summary>Invoked when StarfallStudio is disposed and unavailable.</summary>
public static class Deinitialized
{
    /// <summary> The label. </summary>
    public const string Label = $"StarfallStudio.{nameof(Deinitialized)}";

    /// <summary> Create a new event subscriber. </summary>
    public static StarfallStudioEventSubscriber Subscriber(IDalamudPluginInterface pi, params Action[] actions)
        => new(pi, Label, actions);

    /// <summary> Create a provider. </summary>
    public static StarfallStudioEventProvider Provider(IDalamudPluginInterface pi)
        => new(pi, Label);
}
