namespace GameOffsets.Objects.States
{
    using System;
    using System.Runtime.InteropServices;

    // Ghidra function ref: search for "Abnormal disconnect: "
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct InGameStateOffset
    {
        [FieldOffset(0x290)] public IntPtr AreaInstanceData; // contains area level
        [FieldOffset(0x310)] public IntPtr WorldData; // contains area name
        [FieldOffset(0x340)] public IntPtr UiRootStructPtr; // UiRootStruct
    }
    
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct UiRootStruct
    {
        [FieldOffset(0x5A8)] public IntPtr UiRootPtr; // contains self pointer
        [FieldOffset(0xBF0)] public IntPtr GameUiPtr; // contains self pointer
        [FieldOffset(0xBF8)] public IntPtr GameUiControllerPtr; // contains self pointer
    }
}
