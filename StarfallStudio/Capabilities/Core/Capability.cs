using StarfallStudio.Entities.Core;
using StarfallStudio.UI.Widgets.Core;
using System;

namespace StarfallStudio.Capabilities.Core;

public abstract class Capability(Entity parent) : IDisposable
{
    public Entity Entity { get; } = parent;

    public IWidget? Widget { get; protected set; }

    public virtual void OnEntitySelected()
    {
    }

    public virtual void OnEntityDeselected()
    {
    }

    public virtual void Dispose()
    {
        OnEntityDeselected();

        GC.SuppressFinalize(this);
    }
}
