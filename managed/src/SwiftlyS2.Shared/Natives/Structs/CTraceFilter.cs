using SwiftlyS2.Core.EntitySystem;
using SwiftlyS2.Shared.SchemaDefinitions;
using System.Runtime.InteropServices;

namespace SwiftlyS2.Shared.Natives;

[StructLayout(LayoutKind.Explicit, Pack = 8, Size = 72)]
public struct CTraceFilter
{
    private static bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    [FieldOffset(0x0)] private nint _pVTable;
    [FieldOffset(0x8)] public RnQueryShapeAttr_t QueryShapeAttributes;

    [FieldOffset(0x3A)]
    [MarshalAs(UnmanagedType.U1)]
    internal bool _IterateEntitiesLinux;

    [FieldOffset(0x40)]
    [MarshalAs(UnmanagedType.U1)]
    public bool IterateEntities;

    public CTraceFilter()
    {
        _pVTable = CTraceFilterVTable.pCTraceFilterShouldHitFunctionCall;
        QueryShapeAttributes = new RnQueryShapeAttr_t();
    }
    public CTraceFilter( bool checkIgnoredEntities = true )
    {
        _pVTable = checkIgnoredEntities ? CTraceFilterVTable.pCTraceFilterShouldHitFunctionCall : CTraceFilterVTable.pCTraceFilterVTable;
        QueryShapeAttributes = new RnQueryShapeAttr_t();
    }

    internal void EnsureValid()
    {
        if (this._pVTable == 0)
        {
            _pVTable = CTraceFilterVTable.pCTraceFilterShouldHitFunctionCall;
        }

        if (!IsWindows)
        {
            _IterateEntitiesLinux = IterateEntities;
        }
    }
}

internal static class CTraceFilterVTable
{
    public static nint pCTraceFilterVTable;
    public static nint pCTraceFilterShouldHitFunctionCall;

    [UnmanagedCallersOnly]
    public unsafe static void Destructor( CTraceFilter* filter, byte unk01 )
    {
        // do nothing
    }

    [UnmanagedCallersOnly]
    public unsafe static nint SomeLinuxFunction( CTraceFilter* filter )
    {
        return 0;
    }

    [UnmanagedCallersOnly]
    public static byte ShouldHitEntity()
    {
        return 1;
    }

    [UnmanagedCallersOnly]
    public unsafe static byte ShouldHitEntity( CTraceFilter* filter, nint entity )
    {
        var ent = EntityManager.GetEntityByAddress(entity) as CBaseEntity ?? Helper.AsSchema<CBaseEntity>(entity);
        var entityIndex = ent.Index;

        return ent == null || !ent.IsValid
            ? (byte)0
            : filter->QueryShapeAttributes.EntityIdsToIgnore[0] != entityIndex && filter->QueryShapeAttributes.EntityIdsToIgnore[1] != entityIndex ? (byte)1 : (byte)0;
    }

    static unsafe CTraceFilterVTable()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            pCTraceFilterVTable = Marshal.AllocHGlobal(sizeof(nint) * 2);
            Span<nint> vtable = new((void*)pCTraceFilterVTable, 2);
            vtable[0] = (nint)(delegate* unmanaged< CTraceFilter*, byte, void >)(&Destructor);
            vtable[1] = (nint)(delegate* unmanaged< byte >)(&ShouldHitEntity);

            pCTraceFilterShouldHitFunctionCall = Marshal.AllocHGlobal(sizeof(nint) * 2);
            Span<nint> funcTable = new((void*)pCTraceFilterShouldHitFunctionCall, 2);
            funcTable[0] = (nint)(delegate* unmanaged< CTraceFilter*, byte, void >)(&Destructor);
            funcTable[1] = (nint)(delegate* unmanaged< CTraceFilter*, nint, byte >)(&ShouldHitEntity);
        }
        else
        {
            pCTraceFilterVTable = Marshal.AllocHGlobal(sizeof(nint) * 3);
            Span<nint> vtable = new((void*)pCTraceFilterVTable, 3);
            vtable[0] = (nint)(delegate* unmanaged< CTraceFilter*, byte, void >)(&Destructor);
            vtable[1] = (nint)(delegate* unmanaged< CTraceFilter*, nint >)(&SomeLinuxFunction);
            vtable[2] = (nint)(delegate* unmanaged< byte >)(&ShouldHitEntity);

            pCTraceFilterShouldHitFunctionCall = Marshal.AllocHGlobal(sizeof(nint) * 3);
            Span<nint> funcTable = new((void*)pCTraceFilterShouldHitFunctionCall, 3);
            funcTable[0] = (nint)(delegate* unmanaged< CTraceFilter*, byte, void >)(&Destructor);
            funcTable[1] = (nint)(delegate* unmanaged< CTraceFilter*, nint >)(&SomeLinuxFunction);
            funcTable[2] = (nint)(delegate* unmanaged< CTraceFilter*, nint, byte >)(&ShouldHitEntity);
        }
    }
}