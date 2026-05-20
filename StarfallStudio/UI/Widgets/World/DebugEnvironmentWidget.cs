using StarfallStudio.Capabilities.World;
using StarfallStudio.Game.World;
using StarfallStudio.UI.Controls.Stateless;
using StarfallStudio.UI.Widgets.Core;
using Dalamud.Bindings.ImGui;

namespace StarfallStudio.UI.Widgets.World;

public class DebugEnvironmentWidget(DebugEnvironmentCapability capability) : Widget<DebugEnvironmentCapability>(capability)
{
    public override string HeaderName => "Debug";

    public override WidgetFlags Flags => WidgetFlags.DrawBody;

    override unsafe public void DrawBody()
    {
        if(ImStarfallStudio.ToggelButton("isParticleSystemEnabled", Capability.Environment.isParticleSystemEnabled))
            Capability.Environment.isParticleSystemEnabled = !Capability.Environment.isParticleSystemEnabled;

        if(ImStarfallStudio.ToggelButton("IsEnvironmentOverride", Capability.Environment.IsEnvironmentOverride))
            Capability.Environment.IsEnvironmentOverride = !Capability.Environment.IsEnvironmentOverride;

        if(ImStarfallStudio.ToggelButton("customParticlesEnabled", Capability.Environment.customParticlesEnabled))
            Capability.Environment.customParticlesEnabled = !Capability.Environment.customParticlesEnabled;

        var env = StarfallStudioEnvManager.Instance();
        if(env == null) return;

        ImStarfallStudio.VerticalPadding(5);

        ImGui.SliderFloat("###11"u8, ref env->EnvState.EnvironmentLighting.Unknown1, 0.0f, 100f);
        ImStarfallStudio.AttachToolTip("EnvironmentLighting.Unknown1");
        ImGui.SliderFloat("###21"u8, ref env->EnvState.EnvironmentLighting.Unknown2, 0.0f, 100f);
        ImStarfallStudio.AttachToolTip("EnvironmentLighting.Unknown2");
        ImGui.SliderFloat("###31"u8, ref env->EnvState.EnvironmentLighting.LightDistance, 0.0f, 100f);
        ImStarfallStudio.AttachToolTip("EnvironmentLighting.LightDistance");
        ImGui.SliderFloat("###41"u8, ref env->EnvState.EnvironmentLighting.Unknown4, 0.0f, 100f);
        ImStarfallStudio.AttachToolTip("EnvironmentLighting.Unknown4");

        ImStarfallStudio.VerticalPadding(3);

        ImGui.SliderFloat("###13"u8, ref env->EnvState.Rain.Unknown1, 0.0f, 100f);
        ImStarfallStudio.AttachToolTip("Rain.Unknown1");
        ImGui.SliderFloat("###23"u8, ref env->EnvState.Rain.Unknown2, 0.0f, 100f);
        ImStarfallStudio.AttachToolTip("Rain.Unknown2");
        ImGui.SliderFloat("###33"u8, ref env->EnvState.Rain.Unknown3, 0.0f, 100f);
        ImStarfallStudio.AttachToolTip("Rain.LightDistance");

        ImStarfallStudio.VerticalPadding(3);

        ImGui.SliderFloat("###14"u8, ref env->EnvState.Particles.Unknown1, 0.0f, 100f);
        ImStarfallStudio.AttachToolTip("Particles.Unknown1");

        ImStarfallStudio.VerticalPadding(5);

        ImGui.Text("EnvManager:"u8);
        Capability.DynamisIPC.DrawPointer(&env);

        ImGui.Text("EnvState:"u8);
        Capability.DynamisIPC.DrawPointer(&env->EnvState);

        ImStarfallStudio.VerticalPadding(5);

        ImGui.Text("EnvironmentLighting:"u8);
        Capability.DynamisIPC.DrawPointer(&env->EnvState.EnvironmentLighting);
        ImGui.Text("Stars:"u8);
        Capability.DynamisIPC.DrawPointer(&env->EnvState.Stars);
        ImGui.Text("Fog:"u8);
        Capability.DynamisIPC.DrawPointer(&env->EnvState.Fog);
        ImGui.Text("Clouds:"u8);
        Capability.DynamisIPC.DrawPointer(&env->EnvState.Clouds);
        ImGui.Text("Rain:"u8);
        Capability.DynamisIPC.DrawPointer(&env->EnvState.Rain);
        ImGui.Text("Particles:"u8);
        Capability.DynamisIPC.DrawPointer(&env->EnvState.Particles);
        ImGui.Text("Wind:"u8);
        Capability.DynamisIPC.DrawPointer(&env->EnvState.Wind);
    }
}
