using StarfallStudio.Resources;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures.TextureWraps;

namespace StarfallStudio.UI.Controls.Stateless;

public static partial class ImStarfallStudio
{
    public static void Spinner(ref float angle, float speed = 3.5f)
    {
        angle += ImGui.GetIO().DeltaTime * speed;

        IDalamudTextureWrap img = ResourceProvider.Instance.GetResourceImage("Images.Spinner.png");
        ImageRotated(img, angle);

        if(angle > 360)
        {
            angle = 0;
        }
    }
}
