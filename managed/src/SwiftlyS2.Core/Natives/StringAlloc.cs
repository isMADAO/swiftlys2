using System.Buffers;
using System.Text;
using System.Text.Unicode;

namespace SwiftlyS2.Core.Natives;

public static class StringAlloc
{
    private static readonly ArrayPool<byte> arrayPool = ArrayPool<byte>.Shared;
    public unsafe delegate void CStringAction( byte* cstr );

    public static void CreateCString( string str, int treshold, Action<nint> action )
    {
        var totalByteCount = Encoding.UTF8.GetByteCount(str) + 1;
        // var surpassingTreshold = (treshold > 0 && totalByteCount > treshold) || totalByteCount > 512;

        // if (surpassingTreshold)
        // {
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
        // }
        // else
        // {
        //     unsafe
        //     {
        //         var stringBuffer = stackalloc byte[totalByteCount];
        //         var span = new Span<byte>(stringBuffer, totalByteCount);

        //         _ = Utf8.FromUtf16(str, span, out _, out var bytesWritten);
        //         stringBuffer[totalByteCount - 1] = 0;

        //         action((nint)stringBuffer);
        //     }
        // }
    }

    public static void CreateCString( string str, Action<nint> action )
    {
        CreateCString(str, 256, action);
    }

    public static T CreateCString<T>( string str, int treshold, Func<nint, T> action )
    {
        var totalByteCount = Encoding.UTF8.GetByteCount(str) + 1;
        // var surpassingTreshold = (treshold > 0 && totalByteCount > treshold) || totalByteCount > 512;

        // if (surpassingTreshold)
        // {
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
        // }
        // else
        // {
        //     unsafe
        //     {
        //         var stringBuffer = stackalloc byte[totalByteCount];
        //         var span = new Span<byte>(stringBuffer, totalByteCount);

        //         _ = Utf8.FromUtf16(str, span, out _, out var bytesWritten);
        //         stringBuffer[totalByteCount - 1] = 0;

        //         return action((nint)stringBuffer);
        //     }
        // }
    }

    public static T CreateCString<T>( string str, Func<nint, T> action )
    {
        return CreateCString(str, 256, action);
    }

    public static string CreateCSharpString( int length, int treshold, Action<nint> action )
    {
        var totalByteCount = length + 1;
        // var surpassingTreshold = (treshold > 0 && totalByteCount > treshold) || totalByteCount > 512;

        // if (surpassingTreshold)
        // {
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
        // }
        // else
        // {
        //     unsafe
        //     {
        //         var stringBuffer = stackalloc byte[totalByteCount];
        //         var span = new Span<byte>(stringBuffer, totalByteCount);

        //         action((nint)stringBuffer);
        //         var returnString = Encoding.UTF8.GetString(stringBuffer, totalByteCount - 1);
        //         return returnString;
        //     }
        // }
    }

    public static string CreateCSharpString( int length, Action<nint> action )
    {
        return CreateCSharpString(length, 256, action);
    }
}