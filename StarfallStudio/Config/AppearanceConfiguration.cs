namespace StarfallStudio.Config;

public class AppearanceConfiguration
{
    public ApplyNPCHack ApplyNPCHack { get; set; } = ApplyNPCHack.InGPose;

    public bool EnableTinting { get; set; } = true;

    public bool EnableStarfallStudioStyle { get; set; } = true;

    public bool EnableStarfallStudioColor { get; set; } = true;
    public bool EnableStarfallStudioScale { get; set; } = false;
}
