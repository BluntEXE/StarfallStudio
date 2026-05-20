using StarfallStudio.Resources;
using StarfallStudio.Resources.Sheets;
using OneOf;
using OneOf.Types;

namespace StarfallStudio.Game.Types;

[GenerateOneOf]
public partial class ActionTimelineUnion : OneOfBase<StarfallStudioActionTimeline, None>
{
    public static implicit operator ActionTimelineUnion(ActionTimelineId actionTimelineId)
    {
        if(actionTimelineId.Id != 0 && GameDataProvider.Instance.ActionTimelines.TryGetRow(actionTimelineId.Id, out var timeline))
            return timeline;

        return new None();
    }
}

public record struct ActionTimelineId(ushort Id)
{
    public static ActionTimelineId None { get; } = new(0);

    public static implicit operator ActionTimelineId(ActionTimelineUnion union) => union.Match(
        row => new ActionTimelineId((ushort)row.RowId),
        none => None
    );

    public static implicit operator ActionTimelineId(ushort id) => new(id);
}
