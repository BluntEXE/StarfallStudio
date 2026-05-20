using StarfallStudio.Capabilities.Actor;
using StarfallStudio.Config;
using StarfallStudio.Entities;
using StarfallStudio.Entities.Actor;
using StarfallStudio.Game.Cutscene;
using StarfallStudio.Game.GPose;
using StarfallStudio.Game.Posing;
using StarfallStudio.UI.Controls.Editors;
using Dalamud.Interface.Windowing;
using System;
using System.Numerics;

namespace StarfallStudio.UI.Windows.Specialized;

public class ActionTimelineWindow : Window, IDisposable
{
    private readonly ActionTimelineEditor _editor;
    private readonly EntityManager _entityManager;
    private readonly GPoseService _gPoseService;
    private readonly CutsceneManager _cutsceneManager;

    public ActionTimelineWindow(EntityManager entityManager, CutsceneManager cutsceneManager, GPoseService gPoseService, PhysicsService physicsService, ConfigurationService configurationService) : base($"{StarfallStudio.Name} - ANIMATION CONTROL###brio_action_timelines_window")
    {
        Namespace = "brio_action_timelines_namespace";


        _entityManager = entityManager;
        _gPoseService = gPoseService;
        _cutsceneManager = cutsceneManager;

        _editor = new(_cutsceneManager, gPoseService, entityManager, physicsService, configurationService);

        SizeConstraints = new WindowSizeConstraints
        {
            MaximumSize = new Vector2(270, 5000),
            MinimumSize = new Vector2(430, 350)
        };

        _gPoseService.OnGPoseStateChange += OnGPoseStateChange;
    }

    public override bool DrawConditions()
    {
        if(_entityManager.SelectedEntity is ActorEntity actor && actor.IsProp == true)
        {
            return false;
        }

        if(!_entityManager.SelectedHasCapability<ActionTimelineCapability>())
        {
            return false;
        }

        return base.DrawConditions();
    }

    public override void Draw()
    {
        if(!_entityManager.TryGetCapabilityFromSelectedEntity<ActionTimelineCapability>(out var capability, considerParents: true))
        {
            return;
        }

        WindowName = $"{StarfallStudio.Name} - Animation Control - {capability.Entity.FriendlyName}###brio_action_timelines_window";

        _editor.Draw(true, capability);
    }

    private void OnGPoseStateChange(bool newState)
    {
        if(!newState)
            IsOpen = false;
    }

    public void Dispose()
    {
        _gPoseService.OnGPoseStateChange -= OnGPoseStateChange;
    }
}
