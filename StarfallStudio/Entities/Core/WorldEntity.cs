using Dalamud.Interface;
using System;

namespace StarfallStudio.Entities.Core;

public class WorldEntity : Entity
{
    public override string FriendlyName => "World";
    public override FontAwesomeIcon Icon => FontAwesomeIcon.Globe;
    public override bool IsAttached => true;

    public WorldEntity(IServiceProvider provider) : base("world", provider)
    {
        OnAttached();
    }
}
