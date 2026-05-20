using StarfallStudio.Capabilities.Debug;
using StarfallStudio.Entities.Core;
using Dalamud.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace StarfallStudio.Entities.Debug;

public class DebugEntity(IServiceProvider provider) : Entity(FixedId, provider)
{
    public const string FixedId = "debug_entity";

    public override string FriendlyName => "Debug";
    public override FontAwesomeIcon Icon => FontAwesomeIcon.Bug;

    public override EntityFlags Flags => EntityFlags.AllowOutsideGpose;

    public override void OnAttached()
    {
        AddCapability(ActivatorUtilities.CreateInstance<DebugCapability>(_serviceProvider, this));
    }
}
