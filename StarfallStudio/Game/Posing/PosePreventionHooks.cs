using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.Havok.Animation.Rig;
using System;

namespace StarfallStudio.Game.Posing;

// Prevents the game's animation system from overwriting bone transforms set by Brio.
// Ported from Ktisis PoseHooks.cs. These 6 hooks are the reason Ktisis pose import
// sticks while Brio's native pose import was unreliable.
public unsafe class PosePreventionHooks : IDisposable
{
    // Blocks FFXIV from setting bone model space - return boneId, do nothing
    private delegate ulong SetBoneModelSpaceFfxivDelegate(nint partialSkeleton, ushort boneId, nint transform, bool enableSecondary, bool enablePropagate);
    private readonly Hook<SetBoneModelSpaceFfxivDelegate> _setBoneModelSpaceFfxivHook;

    // Blocks recalculation - return stored model space pointer directly
    private delegate nint CalculateBoneModelSpaceDelegate(ref hkaPose pose, int boneIdx);
    private readonly Hook<CalculateBoneModelSpaceDelegate> _calculateBoneModelSpaceHook;

    // Blocks full model space rebuild/sync
    private delegate void SyncModelSpaceDelegate(hkaPose* pose);
    private readonly Hook<SyncModelSpaceDelegate> _syncModelSpaceHook;

    // Blocks look-at IK (would override head/eye bones)
    private delegate byte* LookAtIKDelegate(byte* a1, long* a2, long* a3, float a4, long* a5, long* a6);
    private readonly Hook<LookAtIKDelegate> _lookAtIKHook;

    // Blocks kinematic driver (physics-driven motion)
    private delegate nint KineDriverDelegate(nint a1, nint a2);
    private readonly Hook<KineDriverDelegate> _kineDriverHook;

    // Reports animation as frozen so the game stops advancing animation frames
    private delegate byte AnimFrozenDelegate(uint* a1, int a2);
    private readonly Hook<AnimFrozenDelegate> _animFrozenHook;

    public PosePreventionHooks(ISigScanner scanner, IGameInteropProvider hooking)
    {
        _setBoneModelSpaceFfxivHook = hooking.HookFromAddress<SetBoneModelSpaceFfxivDelegate>(
            scanner.ScanText("48 8B C4 48 89 58 18 55 56 57 41 54 41 55 41 56 41 57 48 81 EC ?? ?? ?? ?? 0F 29 70 B8 0F 29 78 A8 44 0F 29 40 ?? 44 0F 29 48 ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 8B B1"),
            SetBoneModelSpaceFfxivDetour);

        _calculateBoneModelSpaceHook = hooking.HookFromAddress<CalculateBoneModelSpaceDelegate>(
            scanner.ScanText("40 53 48 83 EC 10 4C 8B 49 28"),
            CalculateBoneModelSpaceDetour);

        _syncModelSpaceHook = hooking.HookFromAddress<SyncModelSpaceDelegate>(
            scanner.ScanText("48 83 EC 18 80 79 38 00"),
            SyncModelSpaceDetour);

        _lookAtIKHook = hooking.HookFromAddress<LookAtIKDelegate>(
            scanner.ScanText("48 8B C4 48 89 58 08 48 89 70 10 F3 0F 11 58"),
            LookAtIKDetour);

        _kineDriverHook = hooking.HookFromAddress<KineDriverDelegate>(
            scanner.ScanText("48 8B C4 55 57 48 83 EC 58"),
            KineDriverDetour);

        _animFrozenHook = hooking.HookFromAddress<AnimFrozenDelegate>(
            scanner.ScanText("E8 ?? ?? ?? ?? 0F B6 F8 84 C0 74 12"),
            AnimFrozenDetour);
    }

    public void Enable()
    {
        _setBoneModelSpaceFfxivHook.Enable();
        _calculateBoneModelSpaceHook.Enable();
        _syncModelSpaceHook.Enable();
        _lookAtIKHook.Enable();
        _kineDriverHook.Enable();
        _animFrozenHook.Enable();
        StarfallStudio.Log.Debug("PosePreventionHooks enabled");
    }

    public void Disable()
    {
        _setBoneModelSpaceFfxivHook.Disable();
        _calculateBoneModelSpaceHook.Disable();
        _syncModelSpaceHook.Disable();
        _lookAtIKHook.Disable();
        _kineDriverHook.Disable();
        _animFrozenHook.Disable();
        StarfallStudio.Log.Debug("PosePreventionHooks disabled");
    }

    // --- Detours ---

    private static ulong SetBoneModelSpaceFfxivDetour(nint partialSkeleton, ushort boneId, nint transform, bool enableSecondary, bool enablePropagate)
    {
        try
        {
            return boneId;
        }
        catch(Exception e)
        {
            StarfallStudio.Log.Error(e, nameof(SetBoneModelSpaceFfxivDetour));
            return boneId;
        }
    }

    private static nint CalculateBoneModelSpaceDetour(ref hkaPose pose, int boneIdx)
    {
        try
        {
            if(pose.ModelPose.Data == null) return nint.Zero;
            return (nint)(pose.ModelPose.Data + boneIdx);
        }
        catch(Exception e)
        {
            StarfallStudio.Log.Error(e, nameof(CalculateBoneModelSpaceDetour));
            return nint.Zero;
        }
    }

    // Intentional no-op — suppresses full model space rebuild so Brio bone transforms persist. Empty body; no try/catch needed.
    private static void SyncModelSpaceDetour(hkaPose* pose)
    { }

    private static byte* LookAtIKDetour(byte* a1, long* a2, long* a3, float a4, long* a5, long* a6)
    {
        try
        {
            return (byte*)nint.Zero;
        }
        catch(Exception e)
        {
            StarfallStudio.Log.Error(e, nameof(LookAtIKDetour));
            return (byte*)nint.Zero;
        }
    }

    private static nint KineDriverDetour(nint a1, nint a2)
    {
        try
        {
            return nint.Zero;
        }
        catch(Exception e)
        {
            StarfallStudio.Log.Error(e, nameof(KineDriverDetour));
            return nint.Zero;
        }
    }

    private static byte AnimFrozenDetour(uint* a1, int a2)
    {
        try
        {
            return 1;
        }
        catch(Exception e)
        {
            StarfallStudio.Log.Error(e, nameof(AnimFrozenDetour));
            return 1;
        }
    }

    public void Dispose()
    {
        Disable();
        _setBoneModelSpaceFfxivHook.Dispose();
        _calculateBoneModelSpaceHook.Dispose();
        _syncModelSpaceHook.Dispose();
        _lookAtIKHook.Dispose();
        _kineDriverHook.Dispose();
        _animFrozenHook.Dispose();
    }
}
