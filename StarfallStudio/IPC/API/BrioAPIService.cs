using StarfallStudio.API;
using StarfallStudio.API.Interface;
using StarfallStudio.Config;
using System;

namespace StarfallStudio.IPC.API;

public class StarfallStudioAPIService(ConfigurationService configurationService, StateAPI stateAPI, ActorAPI actorAPI, EnvironmentAPI environmentAPI, PosingAPI posingAPI, AnimationAPI animationAPI) : IStarfallStudioAPI, IDisposable
{
    private readonly ConfigurationService _configurationService = configurationService;

    public bool IsIPCEnabled => _configurationService.Configuration.IPC.EnableStarfallStudioIPC;


    public bool Valid { get; private set; } = true;


    public IState State { get; } = stateAPI;

    public IActor Actor { get; } = actorAPI;

    public IEnvironment Environment { get; } = environmentAPI;

    public IPosing Posing { get; } = posingAPI;

    public IAnimation Animation { get; } = animationAPI;

    public void Dispose()
    {
        Valid = false;
    }
}
