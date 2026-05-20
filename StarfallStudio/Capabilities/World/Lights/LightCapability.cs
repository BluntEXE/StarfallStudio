using StarfallStudio.Capabilities.Core;
using StarfallStudio.Entities.Core;
using StarfallStudio.Entities.World;
using StarfallStudio.Game.World;
using System;

namespace StarfallStudio.Capabilities.World;

public class LightCapability(Entity parent) : Capability(parent), IDisposable
{
    public LightEntity Light => (LightEntity)Entity;
    public IGameLight GameLight => Light.GameLight;
}
