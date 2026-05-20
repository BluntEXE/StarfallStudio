using StarfallStudio.API.Interface;

namespace StarfallStudio.API;

public interface IStarfallStudioAPI
{
    public bool Valid { get; }


    public IState State { get; }

    public IActor Actor { get; }

    public IEnvironment Environment { get; }

    public IAnimation Animation { get; }

}
