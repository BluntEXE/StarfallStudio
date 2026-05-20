using StarfallStudio.UI.Windows;
using Dalamud.Bindings.ImGui;

namespace StarfallStudio.Library.Sources;

public abstract class SourceBase : GroupEntryBase
{
    public SourceBase()
        : base(null)
    {
    }

    public abstract string Description { get; }
    public abstract void Scan();

    public override void DrawInfo(LibraryWindow window)
    {
        base.DrawInfo(window);

        if(this.Description != null)
        {
            ImGui.TextWrapped(this.Description);
        }
    }
}
