using StarfallStudio.Entities;
using StarfallStudio.Entities.Actor;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.Actor.Extensions;
using StarfallStudio.Game.Core;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System;

namespace StarfallStudio.UI.Controls.Stateless;

public partial class ImStarfallStudio
{
    public static void DrawApplyToActor(EntityManager entityManager, Action<ActorEntity> callback)
    {
        if(entityManager.SelectedEntity is null || entityManager.SelectedEntity is not ActorEntity selectedActor)
        {
            DrawSpawnActor(entityManager, callback);

            return;
        }

        if(ImGui.IsKeyDown(ImGuiKey.LeftCtrl) || ImGui.IsKeyDown(ImGuiKey.RightCtrl))
        {
            DrawSpawnActor(entityManager, callback);
        }
        else
        {
            if(ImGui.Button($"Apply To {selectedActor.FriendlyName}"))
            {
                callback?.Invoke(selectedActor);
            }


            if(ImGui.IsItemHovered())
                ImGui.SetTooltip("Hold Ctrl to spawn as a new actor");
        }

    }

    private static void DrawSpawnActor(EntityManager entityManager, Action<ActorEntity> callback)
    {
        if(!StarfallStudio.TryGetService(out ActorSpawnService spawnService))
        {
            using var _ = ImRaii.Disabled(true);
            ImGui.Button("Unable to Spawn");
        }


        if(ImGui.Button("Spawn As New Actor"))
        {
            if(!spawnService.CreateCharacter(out var character, disableSpawnCompanion: true))
            {
                StarfallStudio.Log.Error("Unable to spawn character");
                return;
            }

            unsafe bool IsReadyToDraw() => character.Native()->IsReadyToDraw();

            StarfallStudio.Framework.RunUntilSatisfied(
                IsReadyToDraw,
                (_) =>
                {
                    var entity = entityManager.GetEntity(new EntityId(character));
                    if(entity is not ActorEntity actorEntity)
                    {
                        StarfallStudio.Log.Error($"Unable to get actor entity is: {entity?.GetType()} {entity}");
                        return;
                    }

                    callback?.Invoke(actorEntity);
                },
                100,
                dontStartFor: 2
            );
        }
    }
}
