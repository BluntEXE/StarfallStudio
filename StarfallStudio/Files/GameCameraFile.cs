using StarfallStudio.Entities.Camera;
using StarfallStudio.Game.Camera;
using MessagePack;
using System;

namespace StarfallStudio.Files;

[Serializable]
[MessagePackObject(keyAsPropertyName: true)]
public class GameCameraFile
{
    [Key(0)] public CameraType CameraType { get; set; }

    [Key(1)] public VirtualCamera? Camera { get; set; }

    [Key(2)] public XATCameraFile? XATCamera { get; set; }
}
