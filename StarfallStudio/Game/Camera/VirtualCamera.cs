using FFXIVClientStructs.FFXIV.Client.Game.Control;
using MessagePack;
using System;
using System.Numerics;

namespace StarfallStudio.Game.Camera;

[MessagePackObject(keyAsPropertyName: true)]
public unsafe partial class VirtualCamera
{
    private readonly Vector3 Up = new(0f, 1f, 0f);

    public VirtualCamera() { }
    public VirtualCamera(int cameraID)
    {
        CameraID = cameraID;

        ResetCamera();
    }

    public FreeCamValues FreeCamValues { get; private set; } = new FreeCamValues();
    public CutsceneCamValues CutsceneCamValues { get; private set; } = new CutsceneCamValues();

    [IgnoreMember] public StarfallStudioCamera* StarfallStudioCamera => (StarfallStudioCamera*)CameraManager.Instance()->GetActiveCamera();

    public unsafe bool IsOverridden => DisableCollision || DelimitCamera || PositionOffset != Vector3.Zero
    | PivotRotation != 0 | FoV != 0 | Pan != Vector2.Zero | StarfallStudioCamera->Camera.Distance != 2.5f;

    public bool HasDelimitOverride => delimitCameraHasOverride;

    [IgnoreMember] public bool IsActiveCamera { get; set; } = false;
    public bool IsFreeCamera { get; set; } = false;
    public bool IsCutsceneCamera { get; set; } = false;

    [IgnoreMember] public int CameraID { get; private set; } = -1;

    public Vector3 SpawnPosition = Vector3.Zero; // TODO (KEN) Implement spawn position logic

    public Vector3 RealPosition => StarfallStudioCamera->GetPosition();

    public Vector3 Position = Vector3.Zero;
    public Vector3 PositionOffset = Vector3.Zero;
    public Vector3 TargetOffset = Vector3.Zero;
    public Vector3 Rotation = Vector3.Zero;

    public bool IsSelectingActor => TargetOffset != Vector3.Zero;
    public string SelectedActorName = "Select an actor to track";

    public float PivotRotation
    {
        get => IsActiveCamera ? StarfallStudioCamera->Roll : (field);
        set
        {
            _ = IsActiveCamera ? (StarfallStudioCamera->Roll = field = value) : (field = value);
        }
    }

    public float Zoom
    {
        get => IsActiveCamera ? StarfallStudioCamera->Camera.Distance : (field);
        set
        {
            _ = IsActiveCamera ? (StarfallStudioCamera->Camera.Distance = field = value) : (field = value);
        }
    } = 0;

    public float FoV
    {
        get => IsActiveCamera ? StarfallStudioCamera->Zoom : (field);
        set
        {
            _ = IsActiveCamera ? (StarfallStudioCamera->Zoom = field = value) : (field = value);
        }
    } = 0;

    public Vector2 Pan
    {
        get => IsActiveCamera ? StarfallStudioCamera->Pan : (field);
        set
        {
            _ = IsActiveCamera ? (StarfallStudioCamera->Pan = field = value) : (field = value);
        }
    }
    public Vector2 Angle
    {
        get => IsActiveCamera ? StarfallStudioCamera->Angle : (field);
        set
        {
            _ = IsActiveCamera ? (StarfallStudioCamera->Angle = field = value) : (field = value);
        }
    }

    public bool DisableCollision = false;

    private Vector2? _originalZoomLimits { get; set; } = null;
    bool delimitCameraHasOverride = false;
    [IgnoreMember]
    public bool DelimitCamera
    {
        get => _originalZoomLimits.HasValue;
        set
        {
            if(IsActiveCamera == false)
                return;

            if(value)
                DelimitCameraStart();
            else
                DelimitCameraStop();

            delimitCameraHasOverride = value;
        }
    }

    public Vector3 RotationAsVector3 => StarfallStudioCamera->RotationAsVector3;

    public Quaternion FreeCameraRotationAsQuaternion
        => Quaternion.CreateFromYawPitchRoll(Rotation.X + MathF.PI - Pan.X, Rotation.Y + Pan.Y, PivotRotation);

    private void DelimitCameraStop()
    {
        if(_originalZoomLimits.HasValue)
        {
            StarfallStudioCamera->Camera.MinDistance = _originalZoomLimits.Value.X;
            StarfallStudioCamera->Camera.MaxDistance = _originalZoomLimits.Value.Y;

            if(StarfallStudioCamera->Camera.Distance < StarfallStudioCamera->Camera.MinDistance)
                StarfallStudioCamera->Camera.Distance = StarfallStudioCamera->Camera.MinDistance;

            _originalZoomLimits = null;
        }
    }
    private void DelimitCameraStart()
    {
        _originalZoomLimits = new Vector2(StarfallStudioCamera->Camera.MinDistance, StarfallStudioCamera->Camera.MaxDistance);

        StarfallStudioCamera->Camera.MinDistance = 0f;
        StarfallStudioCamera->Camera.MaxDistance = 500f;
    }

    public void ActivateCamera()
    {
        if(IsActiveCamera)
            return;

        LoadCameraState();

        IsActiveCamera = true;
    }
    public void DeactivateCamera()
    {
        if(IsActiveCamera is false)
            return;
        IsActiveCamera = false;

        SaveCameraState();
    }

    public unsafe void ResetCamera()
    {
        PositionOffset = Vector3.Zero;

        DisableCollision = false;
        DelimitCamera = false;

        PivotRotation = 0;
        Zoom = 2.5f;
        FoV = 0f;
        Angle = Vector2.Zero;
        Pan = Vector2.Zero;

        SelectedActorName = "Select an actor to track";
        TargetOffset = Vector3.Zero;
    }

    public void SaveCameraState()
    {
        PivotRotation = StarfallStudioCamera->Roll;
        Zoom = StarfallStudioCamera->Camera.Distance;
        FoV = StarfallStudioCamera->Zoom;

        Angle = StarfallStudioCamera->Angle;
        Pan = StarfallStudioCamera->Pan;
    }
    public void LoadCameraState()
    {
        StarfallStudioCamera->Roll = PivotRotation;
        StarfallStudioCamera->Camera.Distance = Zoom;
        StarfallStudioCamera->Zoom = FoV;

        StarfallStudioCamera->Angle = Angle;
        StarfallStudioCamera->Pan = Pan;

        DelimitCamera = delimitCameraHasOverride;
    }

    public void SetCameraID(int id)
    {
        CameraID = id;
    }

    public void ToFreeCam()
    {
        if(Position == Vector3.Zero)
            Position = RealPosition;
        IsFreeCamera = true;
    }
}

[MessagePackObject(keyAsPropertyName: true)]
public class FreeCamValues
{
    public bool IsMovementEnabled = false;
    public bool Move2D = false;

    public float MouseSensitivity { get; set; } = 0f;
    public float MovementSpeed { get; set; } = 0f;

    public bool DelimitAngle = false;
}

[MessagePackObject(keyAsPropertyName: true)]
public class CutsceneCamValues
{
    public bool StartAnimationOnSelect = true;
    public string CameraPath = string.Empty;

    public Vector3 Scale = Vector3.One;
    public Vector3 Offset = Vector3.Zero;
    public bool Loop = false;
    public bool EnableFOV = true;

}
