using StarfallStudio.Capabilities.Core;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.World;
using StarfallStudio.UI.Widgets.World;
using System.Collections.Generic;
using System.Linq;
using static StarfallStudio.Game.World.FestivalService;

namespace StarfallStudio.Capabilities.World;

public class FestivalCapability : Capability
{
    private readonly FestivalService _festivalService;

    public bool CanModify => _festivalService.CanModify;
    public bool CanAdd => _festivalService.HasMoreSlots;
    public bool HasOverride => _festivalService.HasOverride;
    public uint[] ActiveFestivals => [.. _festivalService.EngineActiveFestivals.Select(f => f.Id)];
    public IReadOnlyDictionary<uint, FestivalEntry> AllFestivals => _festivalService.FestivalList;


    public FestivalCapability(Entity parent, FestivalService festivalService) : base(parent)
    {
        _festivalService = festivalService;
        Widget = new FestivalWidget(this);
    }

    public void Add(uint festivalId) => _festivalService.AddFestival(festivalId);
    public void Remove(uint festivalId) => _festivalService.RemoveFestival(festivalId);
    public void Reset() => _festivalService.ResetFestivals();
}
