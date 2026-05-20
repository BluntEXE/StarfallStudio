using StarfallStudio.API.Interface;
using StarfallStudio.Game.GPose;
using StarfallStudio.Game.Posing;

namespace StarfallStudio.IPC.API;

public class EnvironmentAPI(GPoseService gPoseService, PhysicsService physicsService) : IEnvironment
{
    private readonly GPoseService _gPoseService = gPoseService;
    private readonly PhysicsService _physicsService = physicsService;

    public bool FreezePhysics()
    {
        if(_gPoseService.IsGPosing == false) return false;

        return _physicsService.FreezeEnable();
    }

    public bool UnFreezePhysics()
    {
        if(_gPoseService.IsGPosing == false) return false;

        return _physicsService.FreezeRevert();
    }
}
