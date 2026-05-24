using StarfallStudio.Capabilities.Actor;
using StarfallStudio.Capabilities.Posing;
using StarfallStudio.Config;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.Actor.Extensions;
using StarfallStudio.UI.Controls;
using StarfallStudio.UI.Controls.Stateless;
using StarfallStudio.UI.Theming;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace StarfallStudio.Entities.Actor;

public class ActorEntity(IGameObject gameObject, IServiceProvider provider) : Entity(new EntityId(gameObject), provider)
{
    public readonly IGameObject GameObject = gameObject;

    private readonly ConfigurationService _configService = provider.GetRequiredService<ConfigurationService>();

    public string RawName = "";
    public override string FriendlyName
    {
        get
        {
            if(string.IsNullOrEmpty(RawName))
            {
                return _configService.Configuration.Interface.CensorActorNames ? GameObject.GetCensoredName() : GameObject.GetFriendlyName();
            }

            return GameObject.GetAsCustomName(RawName);
        }
        set
        {
            RawName = value;
        }
    }
    public override FontAwesomeIcon Icon => IsProp ? FontAwesomeIcon.Cube : GameObject.GetFriendlyIcon();

    public override bool IsVisible => true;

    // Only show in hierarchy if the parent container considers this actor managed.
    // Sub-children (companions etc.) have an ActorEntity parent, not a container, so they always pass.
    public override bool ShowInHierarchy
    {
        get
        {
            if(Parent is ActorContainerEntity container &&
               container.TryGetCapability<ActorContainerCapability>(out var cap))
                return cap.IsManaged(this);
            return true;
        }
    }

    public override EntityFlags Flags => EntityFlags.AllowDoubleClick | EntityFlags.HasContextButton | EntityFlags.DefaultOpen | EntityFlags.AllowMultiSelect;

    public override int ContextButtonCount => 1;

    public bool IsProp => ActorType == ActorType.Prop;

    public ActorType ActorType => GetActorType();

    private ActorType GetActorType()
    {
        if(SpawnFlag.HasFlag(SpawnFlags.IsEffect))
            return ActorType.Effect;
        if(SpawnFlag.HasFlag(SpawnFlags.IsProp))
            return ActorType.Prop;

        return ActorType.StarfallStudioActor;
    }

    public override void OnDoubleClick()
    {
        var aac = GetCapability<ActorAppearanceCapability>();
        RenameActorModal.Open(aac.Actor);
    }

    public override void DrawContextButton()
    {
        var aac = GetCapability<ActorAppearanceCapability>();

        using(ImRaii.PushColor(ImGuiCol.Button, ThemeManager.CurrentTheme.Accent.AccentColor, aac.IsHidden))
        {
            string toolTip = aac.IsHidden ? $"Show {aac.Actor.FriendlyName}" : $"Hide {aac.Actor.FriendlyName}";
            if(ImStarfallStudio.FontIconButtonRight($"###{Id}_hideActor", aac.IsHidden ? FontAwesomeIcon.EyeSlash : FontAwesomeIcon.Eye, 1f, toolTip, bordered: false))
            {
                aac.ToggleHide();
            }
        }
    }

    public override void OnAttached()
    {
        AddCapability(ActivatorUtilities.CreateInstance<ActorLifetimeCapability>(_serviceProvider, this));
        AddCapability(ActivatorUtilities.CreateInstance<ActorAppearanceCapability>(_serviceProvider, this));

        if(ActorType is ActorType.StarfallStudioActor)
            AddCapability(ActorDynamicPoseCapability.CreateIfEligible(_serviceProvider, this));

        AddCapability(ActivatorUtilities.CreateInstance<SkeletonPosingCapability>(_serviceProvider, this));
        AddCapability(ActivatorUtilities.CreateInstance<ModelPosingCapability>(_serviceProvider, this));
        AddCapability(ActivatorUtilities.CreateInstance<PosingCapability>(_serviceProvider, this));

        AddCapability(ActionTimelineCapability.CreateIfEligible(_serviceProvider, this));

        if(ActorType is not ActorType.Prop)
        {
            if(ActorType is not ActorType.Effect)
            {
                AddCapability(CompanionCapability.CreateIfEligible(_serviceProvider, this));
            }

            AddCapability(StatusEffectCapability.CreateIfEligible(_serviceProvider, this));
        }

        AddCapability(ActivatorUtilities.CreateInstance<ActorDebugCapability>(_serviceProvider, this));

        // After all capabilities are ready, let the container hide ambient (non-managed) actors.
        if(Parent is ActorContainerEntity container &&
           container.TryGetCapability<ActorContainerCapability>(out var cap))
            cap.HideIfAmbient(this);
    }
}
