using System.Reflection;
using System.Runtime.InteropServices;
using Semver;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2;

[StructLayout(LayoutKind.Sequential)]
public struct CGcBanInformation_t
{
    public uint Reason;
    public double Unknown;
    public double Expiration;
    public uint AccountId;
}

[StructLayout(LayoutKind.Explicit, Pack = 8, Size = 0x48)]
public unsafe struct TraceFilters
{
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

}

public class Program
{
    public static void Main()
    {
        PrintPadding<TraceFilters>();
    }

    public static void PrintPadding<T>()
    {
        var fields = typeof(T).GetFields(BindingFlags.Instance |
                                         BindingFlags.Public |
                                         BindingFlags.NonPublic);

        Array.Sort(fields, ( a, b ) =>
            Marshal.OffsetOf<T>(a.Name).ToInt32()
            .CompareTo(Marshal.OffsetOf<T>(b.Name).ToInt32()));

        var current = 0;

        foreach (var field in fields)
        {
            var offset = Marshal.OffsetOf<T>(field.Name).ToInt32();

            if (offset > current)
                Console.WriteLine($"Padding: {current} - {offset - 1}");

            Console.WriteLine($"{field.Name}: {offset}");

            current = offset + Marshal.SizeOf(field.FieldType);
        }

        var total = Marshal.SizeOf<T>();

        if (current < total)
            Console.WriteLine($"Padding: {current} - {total - 1}");
    }

    private static int GetFieldSize( Type type )
    {
        if (type.IsPointer)
            return IntPtr.Size;
        else if (type.IsEnum)
            return GetFieldSize(Enum.GetUnderlyingType(type));

        try
        {
            return Marshal.SizeOf(type);
        }
        catch
        {
            return 0;
        }
    }
}