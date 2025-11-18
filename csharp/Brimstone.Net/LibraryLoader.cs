using System.Runtime.InteropServices;

namespace Brimstone.Net;

/// <summary>
/// Helper class for loading the native Brimstone library
/// </summary>
internal static class LibraryLoader
{
    static LibraryLoader()
    {
        NativeLibrary.SetDllImportResolver(typeof(NativeMethods).Assembly, DllImportResolver);
    }

    private static IntPtr DllImportResolver(string libraryName, System.Reflection.Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName != "brimstone_ffi")
            return IntPtr.Zero;

        // Try to load from the same directory as the assembly
        var assemblyDir = Path.GetDirectoryName(assembly.Location);
        if (assemblyDir != null)
        {
            var libPath = GetLibraryPath(assemblyDir);
            if (File.Exists(libPath) && NativeLibrary.TryLoad(libPath, out var handle))
            {
                return handle;
            }
        }

        // Try to load from current directory
        var currentDir = Directory.GetCurrentDirectory();
        var currentDirPath = GetLibraryPath(currentDir);
        if (File.Exists(currentDirPath) && NativeLibrary.TryLoad(currentDirPath, out var currentHandle))
        {
            return currentHandle;
        }

        // Try to load from ../../target/release (for development)
        var devPath = Path.Combine(currentDir, "..", "..", "target", "release");
        if (Directory.Exists(devPath))
        {
            var devLibPath = GetLibraryPath(devPath);
            if (File.Exists(devLibPath) && NativeLibrary.TryLoad(devLibPath, out var devHandle))
            {
                return devHandle;
            }
        }

        // Let the default resolution handle it
        return IntPtr.Zero;
    }

    private static string GetLibraryPath(string directory)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Path.Combine(directory, "brimstone_ffi.dll");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return Path.Combine(directory, "libbrimstone_ffi.dylib");
        }
        else
        {
            return Path.Combine(directory, "libbrimstone_ffi.so");
        }
    }

    /// <summary>
    /// Call this to ensure the static constructor runs
    /// </summary>
    public static void EnsureLoaded()
    {
        // Nothing to do, just ensuring static constructor runs
    }
}
