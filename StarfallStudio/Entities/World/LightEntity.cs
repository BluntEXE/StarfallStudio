using StarfallStudio.Capabilities.World;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.World;
using StarfallStudio.UI.Controls.Stateless;
using StarfallStudio.UI.Theming;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace StarfallStudio.Entities.World;

public class LightEntity(IGameLight gameLight, IServiceProvider provider) : Entity(new EntityId(gameLight), provider)
{
    public string RawName = "";
    public override string FriendlyName
    {
        get
        {
            var indexName = 1 + GameLight.Index;

            if(string.IsNullOrEmpty(RawName))
            {
                if(GameLight.IsGPoseLight)
                    return $"GPose Light ({indexName})";

                return $"Light ({indexName})";
            }

            return $"{RawName} ({indexName})";
        }
        set
        {
            RawName = value;
        }
    }

    public override int ContextButtonCount => 1;

    public unsafe override bool IsVisible => true;

    public override EntityFlags Flags => EntityFlags.AllowDoubleClick | EntityFlags.HasContextButton | EntityFlags.DefaultOpen;

    public override FontAwesomeIcon Icon => FontAwesomeIcon.Lightbulb;

    public IGameLight GameLight => gameLight;

    public override unsafe void DrawContextButton()
    {
        using(ImRaii.PushColor(ImGuiCol.Button, ThemeManager.CurrentTheme.Accent.AccentColor, GameLight.IsVisible))
        {
            string toolTip = !GameLight.IsVisible ? $"Show {FriendlyName}" : $"Hide {FriendlyName}";
            if(ImStarfallStudio.FontIconButtonRight($"###{Id}_hideLight", !GameLight.IsVisible ? FontAwesomeIcon.EyeSlash : FontAwesomeIcon.Eye, 1f, toolTip, bordered: false))
            {
                GameLight.ToggleLight();
            }
        }
    }

    public override void OnAttached()
    {
        AddCapability(ActivatorUtilities.CreateInstance<LightLifetimeCapability>(_serviceProvider, this));
        AddCapability(ActivatorUtilities.CreateInstance<LightTransformCapability>(_serviceProvider, this));
        AddCapability(ActivatorUtilities.CreateInstance<LightRenderingCapability>(_serviceProvider, this));

        AddCapability(ActivatorUtilities.CreateInstance<LightDebugCapability>(_serviceProvider, this));
    }
}
