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

[StructLayout(LayoutKind.Explicit, Pack = 8, Size = 96)]
public unsafe struct CTraceFilter
{
    private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    private static readonly int ShouldHitEntityVTableIndex = IsWindows ? 1 : 2;

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

    [FieldOffset(0x48)]
    internal nint CustomShouldHitEntityFuncPtr;

    public ShouldHitEntityDelegate? ShouldHitEntity {
        set {
            if (value == null)
            {
                CustomShouldHitEntityFuncPtr = nint.Zero;
                return;
            }

            CustomShouldHitEntityFuncPtr = Marshal.GetFunctionPointerForDelegate(value!);
        }
    }

    [FieldOffset(0x50)]
    internal nint vTableName;

    [FieldOffset(0x58)]
    internal byte CheckIgnoredEntities;

    [FieldOffset(0x60)]
    internal delegate* unmanaged< CTraceFilter*, nint, byte > OriginalShouldHitEntityFuncPtr;

    public CTraceFilter() : this(true) { }

    public CTraceFilter( bool checkIgnoredEntities = true ) : this("CTraceFilter")
    {
        CheckIgnoredEntities = (byte)(checkIgnoredEntities ? 1 : 0);
    }

    public CTraceFilter( string? vTableName )
    {
        this.vTableName = Marshal.StringToHGlobalAnsi(vTableName ?? "CTraceFilter");

        EnsureValid();
        QueryShapeAttributes = new RnQueryShapeAttr_t();
    }

    internal void EnsureValid()
    {
        if (this._pVTable == null)
        {
            _pVTable = (nint*)NativeMemory.Alloc((nuint)((IsWindows ? 2 : 3) * sizeof(nint)));

            var vTable = (nint*)NativeMemoryHelpers.GetVirtualTableAddress("server", Marshal.PtrToStringAnsi(this.vTableName) ?? "CTraceFilter");
            for (var i = 0; i < (IsWindows ? 2 : 3); i++)
            {
                _pVTable[i] = vTable[i];
            }

            OriginalShouldHitEntityFuncPtr = (delegate* unmanaged< CTraceFilter*, nint, byte >)_pVTable[ShouldHitEntityVTableIndex];
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
    }

    [UnmanagedCallersOnly]
    private static byte ShouldHitEntityHook( CTraceFilter* filter, nint entityPtr )
    {
        var entity = EntityManager.GetEntityByAddress(entityPtr) ?? Helper.AsSchema<CEntityInstance>(entityPtr);

        if (filter->CheckIgnoredEntities == 1)
        {
            if (!entity.IsValid) return 0;

            var entityIndex = entity.Index;
            if (filter->QueryShapeAttributes.EntityIdsToIgnore[0] == entityIndex || filter->QueryShapeAttributes.EntityIdsToIgnore[1] == entityIndex)
            {
                return 0;
            }
        }

        if (filter->CustomShouldHitEntityFuncPtr != 0)
        {
            var shouldHitEntityFunc = Marshal.GetDelegateForFunctionPointer<ShouldHitEntityDelegate>(filter->CustomShouldHitEntityFuncPtr);
            return shouldHitEntityFunc(entity) ? (byte)1 : (byte)0;
        }

        return filter->OriginalShouldHitEntityFuncPtr(filter, entityPtr);
    }
}
