using System.Reflection;
using System.Runtime.InteropServices;
using Spectre.Console;

namespace SwiftlyS2.Core.Natives;

internal class NativeBinding
{
    public static bool IsMainThread => NativeCore.IsMainThread();

    public static void ThrowIfNonMainThread()
    {
        if (!IsMainThread)
        {
            throw new InvalidOperationException("This method can only be called from the main thread.");
        }
    }

    public static void BindNatives( IntPtr nativeTable, int nativeTableSize )
    {
        unsafe
        {
            try
            {
                var pNativeTables = (NativeFunction*)nativeTable;

                for (var i = 0; i < nativeTableSize; i++)
                {
                    var name = Marshal.PtrToStringUTF8(pNativeTables[i].Name)!;

                    var names = name.Split('.');
                    var className = names[0];
                    var funcName = names[1];

                    var nativeNameSpace = "SwiftlyS2.Core.Natives.Native" + className;

                    var nativeClass = Type.GetType(nativeNameSpace)!;
                    var nativeStaticField = nativeClass.GetField("_" + funcName,
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    nativeStaticField!.SetValue(null, pNativeTables[i].Function);
                }
            }
            catch (Exception e)
            {
                if (!GlobalExceptionHandler.Handle(ref e)) return;
                AnsiConsole.WriteException(e);
            }
        }
    }
}
