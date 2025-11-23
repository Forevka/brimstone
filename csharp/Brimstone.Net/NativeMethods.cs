using System.Runtime.InteropServices;

namespace Brimstone.Net;

/// <summary>
/// P/Invoke declarations for the Brimstone FFI
/// </summary>
internal static class NativeMethods
{
    private const string LibName = "brimstone_ffi";

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void bs_init();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr bs_context_new();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void bs_context_free(IntPtr ctx);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern BsResultCode bs_eval(
        IntPtr ctx,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string code,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string? filename,
        out IntPtr errorOut);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern BsResultCode bs_eval_with_result(
        IntPtr ctx,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string code,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string? filename,
        out IntPtr resultOut,
        out IntPtr errorOut);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void bs_string_free(IntPtr s);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr bs_version();
}
