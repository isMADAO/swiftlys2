using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;

namespace SwiftlyS2.Core.Natives;

internal static class StringAlloc
{
    private static readonly ArrayPool<byte> arrayPool = ArrayPool<byte>.Shared;
    public unsafe delegate void CStringAction( byte* cstr );

    public static void CreateCString( string str, int treshold, Action<nint> action )
    {
        var totalByteCount = Encoding.UTF8.GetByteCount(str) + 1;
        var stringBuffer = arrayPool.Rent(totalByteCount);

        _ = Utf8.FromUtf16(str, stringBuffer, out _, out var bytesWritten);
        stringBuffer[totalByteCount - 1] = 0;

        unsafe
        {
            fixed (byte* cstr = stringBuffer)
            {
                action((nint)cstr);
            }
        }

        arrayPool.Return(stringBuffer);
    }

    public static void CreateCString( string str, Action<nint> action )
    {
        CreateCString(str, 256, action);
    }

    public static T CreateCString<T>( string str, int treshold, Func<nint, T> action )
    {
        var totalByteCount = Encoding.UTF8.GetByteCount(str) + 1;
        var stringBuffer = arrayPool.Rent(totalByteCount);

        _ = Utf8.FromUtf16(str, stringBuffer, out _, out var bytesWritten);
        stringBuffer[totalByteCount - 1] = 0;

        unsafe
        {
            fixed (byte* cstr = stringBuffer)
            {
                var result = action((nint)cstr);
                arrayPool.Return(stringBuffer);
                return result;
            }
        }
    }

    public static T CreateCString<T>( string str, Func<nint, T> action )
    {
        return CreateCString(str, 256, action);
    }

    public static string CreateCSharpString( int length, int treshold, Action<nint> action )
    {
        var totalByteCount = length + 1;
        var stringBuffer = arrayPool.Rent(totalByteCount);

        unsafe
        {
            fixed (byte* cstr = stringBuffer)
            {
                action((nint)cstr);
                var returnString = Encoding.UTF8.GetString(cstr, totalByteCount - 1);
                arrayPool.Return(stringBuffer);
                return returnString;
            }
        }
    }

    public static string CreateCSharpString( int length, Action<nint> action )
    {
        return CreateCSharpString(length, 256, action);
    }

    public static string CreateCSharpString( nint cstrPtr )
    {
        return Marshal.PtrToStringUTF8(cstrPtr) ?? "(null)";
    }
}