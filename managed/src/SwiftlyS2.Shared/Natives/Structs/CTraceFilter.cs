using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using SwiftlyS2.Core.EntitySystem;
using SwiftlyS2.Core.Natives;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace SwiftlyS2.Shared.Natives;

public delegate bool ShouldHitEntityDelegate( CEntityInstance entity );

public static class CTraceFilterPreset
{
    public static CTraceFilter Default => new();
    public static CTraceFilter CheckIgnoredEntities => new(checkIgnoredEntities: true);
    public static CTraceFilter WalkableEntities => new("CTraceFilterWalkableEntities");
    public static CTraceFilter NoPlayersAndFlashbangPassableAnims => new("CTraceFilterNoPlayersAndFlashbangPassableAnims");
    public static CTraceFilter IgnoreGrenades => new("CTraceFilterIgnoreGrenades");
    public static CTraceFilter KnifeIgnoreTeammates => new("CTraceFilterKnifeIgnoreTeammates");
    public static CTraceFilter TaserIgnoreTeammates => new("CTraceFilterTaserIgnoreTeammates");
    public static CTraceFilter PlayerMovementCS => new("CTraceFilterPlayerMovementCS");
    public static CTraceFilter ForPlayerHeadCollision => new("CTraceFilterForPlayerHeadCollision");
    public static CTraceFilter LOS => new("CTraceFilterLOS");
    public static CTraceFilter List => new("CTraceFilterList");
    public static CTraceFilter EntitySweep => new("CTraceFilterEntitySweep");
    public static CTraceFilter EntityPush => new("CTraceFilterEntityPush");
    public static CTraceFilter PushMove => new("CTraceFilterPushMove");
    public static CTraceFilter PushFinal => new("CTraceFilterPushFinal");
    public static CTraceFilter Door => new("CTraceFilterDoor");
    public static CTraceFilter QueryCache => new("CTraceFilterQueryCache");
    public static CTraceFilter GroundEntities => new("CTraceFilterGroundEntities");
}

internal class TraceFilterData
{
    public ShouldHitEntityDelegate? CustomShouldHitEntityFunc;
    public string VTableName = "CTraceFilter";
    public bool CheckIgnoredEntities = true;
    public unsafe delegate* unmanaged< CTraceFilter*, nint, byte > OriginalShouldHitEntityFuncPtr;
    public bool ManualDispose = false;
    public nint VTablePtr = 0;
}

internal static class CTraceFilterData
{
    public static ConcurrentDictionary<uint, TraceFilterData> CTraceFilterDataMap = [];
    private static uint TraceFilterID = 1;
    public static uint GetTraceFilterID()
    {
        if (TraceFilterID % 1_000_000 == 0) TraceFilterID = 1;

        return TraceFilterID++;
    }
}

[StructLayout(LayoutKind.Explicit, Pack = 8, Size = 0x48)]
public unsafe struct CTraceFilter
{
    private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    private static readonly int ShouldHitEntityVTableIndex = IsWindows ? 1 : 2;
    private static readonly int VTableSize = IsWindows ? 2 : 3;

    [FieldOffset(0x0)]
    private nint* _pVTable;

    [FieldOffset(0x8)]
    public RnQueryShapeAttr_t QueryShapeAttributes;

    [FieldOffset(0x3A)]
    [MarshalAs(UnmanagedType.U1)]
    private bool _IterateEntities_Linux;

    [FieldOffset(0x40)]
    [MarshalAs(UnmanagedType.U1)]
    public bool IterateEntities;

    [FieldOffset(0x44)]
    [MarshalAs(UnmanagedType.U4)]
    public uint TraceFilterID;

    public ShouldHitEntityDelegate? ShouldHitEntity {
        set {
            var data = GetTraceFilterData();
            data.CustomShouldHitEntityFunc = value;
        }
    }

    public bool ManualDispose {
        get {
            var data = GetTraceFilterData();
            return data.ManualDispose;
        }
        set {
            var data = GetTraceFilterData();
            data.ManualDispose = value;
        }
    }

    public CTraceFilter() : this(true) { }

    public CTraceFilter( bool checkIgnoredEntities = true ) : this("CTraceFilter")
    {
        EnsureTraceFilterData();
        var data = GetTraceFilterData();
        data.CheckIgnoredEntities = checkIgnoredEntities;
    }

    public CTraceFilter( string? vTableName )
    {
        EnsureTraceFilterData();
        var data = GetTraceFilterData();
        data.VTableName = vTableName ?? "CTraceFilter";
        data.VTablePtr = NativeMemoryHelpers.GetVirtualTableAddress("server", vTableName ?? "CTraceFilter");

        EnsureValid();
        QueryShapeAttributes = new RnQueryShapeAttr_t();
    }

    internal void EnsureTraceFilterData()
    {
        if (!CTraceFilterData.CTraceFilterDataMap.ContainsKey(TraceFilterID))
        {
            var traceFilterId = CTraceFilterData.GetTraceFilterID();
            TraceFilterID = traceFilterId;
            _ = CTraceFilterData.CTraceFilterDataMap.TryAdd(TraceFilterID, new TraceFilterData());
        }
    }

    internal TraceFilterData GetTraceFilterData()
    {
        if (!CTraceFilterData.CTraceFilterDataMap.ContainsKey(TraceFilterID))
        {
            EnsureTraceFilterData();
        }

        return CTraceFilterData.CTraceFilterDataMap.TryGetValue(TraceFilterID, out var data) ? data : new TraceFilterData();
    }

    internal void EnsureValid()
    {
        if (this._pVTable == null)
        {
            var traceFilterData = GetTraceFilterData();
            _pVTable = (nint*)NativeMemory.Alloc((nuint)(VTableSize * sizeof(nint)));

            var vTable = (nint*)traceFilterData.VTablePtr;
            for (var i = 0; i < VTableSize; i++)
            {
                _pVTable[i] = vTable[i];
            }

            traceFilterData.OriginalShouldHitEntityFuncPtr = (delegate* unmanaged< CTraceFilter*, nint, byte >)_pVTable[ShouldHitEntityVTableIndex];
            unsafe
            {
                _pVTable[ShouldHitEntityVTableIndex] = (nint)(delegate* unmanaged< CTraceFilter*, nint, byte >)&ShouldHitEntityHook;
            }
        }

        if (!IsWindows)
        {
            _IterateEntities_Linux = IterateEntities;
        }
    }

    public bool ShouldIgnoreEntity( CEntityInstance ent )
    {
        if (ent == null || !ent.IsValid) return false;

        var entityIndex = ent.Index;
        return QueryShapeAttributes.EntityIdsToIgnore[0] == entityIndex || QueryShapeAttributes.EntityIdsToIgnore[1] == entityIndex;
    }

    internal void Dispose()
    {
        if (this._pVTable != null)
        {
            NativeMemory.Free(this._pVTable);
            this._pVTable = null;
        }

        _ = CTraceFilterData.CTraceFilterDataMap.TryRemove(TraceFilterID, out _);
    }

    [UnmanagedCallersOnly]
    private static byte ShouldHitEntityHook( CTraceFilter* filter, nint entityPtr )
    {
        var entity = EntityManager.GetEntityByAddress(entityPtr) ?? Helper.AsSchema<CEntityInstance>(entityPtr);
        var traceFilterData = filter->GetTraceFilterData();

        if (traceFilterData.CheckIgnoredEntities)
        {
            if (!entity.IsValid) return 0;

            var entityIndex = entity.Index;
            if (filter->QueryShapeAttributes.EntityIdsToIgnore[0] == entityIndex || filter->QueryShapeAttributes.EntityIdsToIgnore[1] == entityIndex)
            {
                return 0;
            }
        }

        if (traceFilterData.CustomShouldHitEntityFunc != null)
        {
            return traceFilterData.CustomShouldHitEntityFunc(entity) ? (byte)1 : (byte)0;
        }

        return traceFilterData.OriginalShouldHitEntityFuncPtr(filter, entityPtr);
    }
}
