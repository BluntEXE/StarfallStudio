using StarfallStudio.Capabilities.Posing;
using StarfallStudio.Entities;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.Actor.Extensions;
using StarfallStudio.Game.GPose;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using System;
using static StarfallStudio.Game.Actor.ActorRedrawService;
using StructsGameObject = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;

namespace StarfallStudio.Game.Posing;

public unsafe class ModelTransformService : IDisposable
{
    public delegate void SetPositionDelegate(StructsGameObject* gameObject, float x, float y, float z);
    private readonly Hook<SetPositionDelegate> _setPositionHook = null!;

    public delegate void SetRotationDelegate(StructsGameObject* gameObject, float rotation);
    private readonly Hook<SetRotationDelegate> _setRotationHook = null!;

    private readonly EntityManager _entityManager;
    private readonly GPoseService _gPoseService;
    private readonly ActorRedrawService _actorRedrawService;

    public ModelTransformService(EntityManager entityManager, GPoseService gPoseService, ActorRedrawService actorRedrawService, IGameInteropProvider hooking)
    {
        _entityManager = entityManager;
        _gPoseService = gPoseService;
        _actorRedrawService = actorRedrawService;

        _setPositionHook = hooking.HookFromAddress<SetPositionDelegate>((nint)StructsGameObject.Addresses.SetPosition.Value, UpdatePositionDetour);
        _setPositionHook.Enable();

        _setRotationHook = hooking.HookFromAddress<SetRotationDelegate>((nint)StructsGameObject.Addresses.SetRotation.Value, UpdateRotationDetour);
        _setRotationHook.Enable();

        _actorRedrawService.ActorRedrawEvent += OnActorRedraw;
    }

    public unsafe Transform GetTransform(IGameObject go)
    {
        var native = go.Native();
        var drawObject = native->DrawObject;
        if(drawObject != null)
        {
            return *(Transform*)(&drawObject->Object.Position);
        }
        else
        {
            return new Transform()
            {
                Position = native->Position
            };
        }
        ;
    }

    public unsafe void SetTransform(IGameObject go, Transform transform) => SetTransform(go.Native(), transform);

    public unsafe void SetTransform(StructsGameObject* native, Transform transform)
    {
        var drawObject = native->DrawObject;

        if(drawObject != null)
        {
            *(Transform*)(&drawObject->Object.Position) = transform;
        }
    }

    private void UpdatePositionDetour(StructsGameObject* gameObject, float x, float y, float z)
    {
        try
        {
            if(_gPoseService.IsGPosing)
            {
                if(_entityManager.TryGetEntity(gameObject, out var entity))
                {
                    if(entity.TryGetCapability<ModelPosingCapability>(out var transformCapability))
                    {
                        if(transformCapability.OverrideTransform.HasValue)
                        {
                            var transform = transformCapability.OverrideTransform.Value;
                            SetTransform(gameObject, transform);
                            return;
                        }
                    }
                }
            }

            _setPositionHook.Original(gameObject, x, y, z);
        }
        catch(Exception e)
        {
            StarfallStudio.Log.Error(e, nameof(UpdatePositionDetour));
            _setPositionHook.Original(gameObject, x, y, z);
        }
    }

    private void UpdateRotationDetour(StructsGameObject* gameObject, float rotation)
    {
        try
        {
            // Always call original first -- skipping it during EnableDraw corrupts
            // the game's model matrix before DrawObject exists, causing an AV.
            _setRotationHook.Original(gameObject, rotation);

            if(_gPoseService.IsGPosing)
            {
                if(_entityManager.TryGetEntity(gameObject, out var entity))
                {
                    if(entity.TryGetCapability<ModelPosingCapability>(out var transformCapability))
                    {
                        if(transformCapability.OverrideTransform.HasValue)
                        {
                            SetTransform(gameObject, transformCapability.OverrideTransform.Value);
                        }
                    }
                }
            }
        }
        catch(Exception e)
        {
            StarfallStudio.Log.Error(e, nameof(UpdateRotationDetour));
        }
    }

    private void OnActorRedraw(IGameObject go, RedrawStage stage)
    {
        if(go is not null)
            if(stage == RedrawStage.After)
                UpdatePositionDetour((StructsGameObject*)go.Address, go.Position.X, go.Position.Y, go.Position.Z);
    }


    public void Dispose()
    {
        _setPositionHook.Dispose();
        _setRotationHook.Dispose();
        _actorRedrawService.ActorRedrawEvent -= OnActorRedraw;
    }
}
