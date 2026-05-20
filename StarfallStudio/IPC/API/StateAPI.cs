using StarfallStudio.API.Interface;
using StarfallStudio.Game.GPose;

namespace StarfallStudio.IPC.API;

public class StateAPI(GPoseService gPoseService) : IState
{

    private readonly GPoseService _gPoseService = gPoseService;

    public (int Breaking, int Feature) ApiVersion => (StarfallStudio.MajorAPIVersion, StarfallStudio.MinorAPIVersion);

    public bool IsAvailable => true;

    public bool IsValidGPoseSession => _gPoseService.IsGPosing;
}
