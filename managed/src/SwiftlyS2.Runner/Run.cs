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

public class Program
{
    public static void Main()
    {
        PrintStructInfo<CTraceFilter>(0);
        PrintStructInfo<RnQueryShapeAttr_t>(0);
    }

    private static void PrintStructInfo<T>( int baseOffset ) where T : struct
    {
        var fields = typeof(T).GetFields(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance
        );

        foreach (var field in fields)
        {
            var offset = Marshal.OffsetOf<T>(field.Name) + baseOffset;
            var size = GetFieldSize(field.FieldType);
            Console.WriteLine($"{field.Name,-40} Offset: 0x{offset:X4} ({offset,4})  Size: {size,4} bytes");
        }

        Console.WriteLine($"\nTotal struct size: {Marshal.SizeOf(typeof(T))} bytes (0x{Marshal.SizeOf(typeof(T)):X} hex)");
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