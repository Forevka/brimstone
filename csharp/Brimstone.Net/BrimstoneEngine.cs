using System.Runtime.InteropServices;

namespace Brimstone.Net;

/// <summary>
/// Represents a Brimstone JavaScript engine instance
/// </summary>
public class BrimstoneEngine : IDisposable
{
    private IntPtr _context;
    private bool _disposed = false;
    private static bool _initialized = false;
    private static readonly object _initLock = new object();

    /// <summary>
    /// Initialize the Brimstone runtime. This must be called before creating any engine instances.
    /// </summary>
    public static void Initialize()
    {
        lock (_initLock)
        {
            if (!_initialized)
            {
                LibraryLoader.EnsureLoaded();
                NativeMethods.bs_init();
                _initialized = true;
            }
        }
    }

    /// <summary>
    /// Gets the Brimstone version string
    /// </summary>
    public static string Version
    {
        get
        {
            var ptr = NativeMethods.bs_version();
            return Marshal.PtrToStringAnsi(ptr) ?? "Unknown";
        }
    }

    /// <summary>
    /// Creates a new Brimstone JavaScript engine instance
    /// </summary>
    public BrimstoneEngine()
    {
        Initialize();

        _context = NativeMethods.bs_context_new();
        if (_context == IntPtr.Zero)
        {
            throw new BrimstoneException("Failed to create Brimstone context", BsResultCode.Error);
        }
    }

    /// <summary>
    /// Evaluates JavaScript code
    /// </summary>
    /// <param name="code">The JavaScript code to evaluate</param>
    /// <param name="filename">Optional filename for error messages</param>
    /// <exception cref="BrimstoneException">Thrown if evaluation fails</exception>
    public void Eval(string code, string? filename = null)
    {
        ThrowIfDisposed();

        IntPtr errorPtr = IntPtr.Zero;
        try
        {
            var result = NativeMethods.bs_eval(_context, code, filename, out errorPtr);

            if (result != BsResultCode.Success)
            {
                string errorMessage = "JavaScript evaluation failed";
                if (errorPtr != IntPtr.Zero)
                {
                    errorMessage = Marshal.PtrToStringUTF8(errorPtr) ?? errorMessage;
                }
                throw new BrimstoneException(errorMessage, result);
            }
        }
        finally
        {
            if (errorPtr != IntPtr.Zero)
            {
                NativeMethods.bs_string_free(errorPtr);
            }
        }
    }

    /// <summary>
    /// Evaluates JavaScript code and returns the result as a string
    /// </summary>
    /// <param name="code">The JavaScript code to evaluate</param>
    /// <param name="filename">Optional filename for error messages</param>
    /// <returns>The result of the evaluation as a string</returns>
    /// <exception cref="BrimstoneException">Thrown if evaluation fails</exception>
    public string EvalWithResult(string code, string? filename = null)
    {
        ThrowIfDisposed();

        IntPtr resultPtr = IntPtr.Zero;
        IntPtr errorPtr = IntPtr.Zero;
        try
        {
            var result = NativeMethods.bs_eval_with_result(_context, code, filename, out resultPtr, out errorPtr);

            if (result != BsResultCode.Success)
            {
                string errorMessage = "JavaScript evaluation failed";
                if (errorPtr != IntPtr.Zero)
                {
                    errorMessage = Marshal.PtrToStringUTF8(errorPtr) ?? errorMessage;
                }
                throw new BrimstoneException(errorMessage, result);
            }

            return Marshal.PtrToStringUTF8(resultPtr) ?? string.Empty;
        }
        finally
        {
            if (resultPtr != IntPtr.Zero)
            {
                NativeMethods.bs_string_free(resultPtr);
            }
            if (errorPtr != IntPtr.Zero)
            {
                NativeMethods.bs_string_free(errorPtr);
            }
        }
    }

    /// <summary>
    /// Evaluates JavaScript code from a file
    /// </summary>
    /// <param name="filePath">Path to the JavaScript file</param>
    /// <exception cref="BrimstoneException">Thrown if evaluation fails</exception>
    public void EvalFile(string filePath)
    {
        var code = File.ReadAllText(filePath);
        Eval(code, filePath);
    }

    /// <summary>
    /// Evaluates JavaScript code from a file and returns the result
    /// </summary>
    /// <param name="filePath">Path to the JavaScript file</param>
    /// <returns>The result of the evaluation as a string</returns>
    /// <exception cref="BrimstoneException">Thrown if evaluation fails</exception>
    public string EvalFileWithResult(string filePath)
    {
        var code = File.ReadAllText(filePath);
        return EvalWithResult(code, filePath);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(BrimstoneEngine));
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (_context != IntPtr.Zero)
            {
                NativeMethods.bs_context_free(_context);
                _context = IntPtr.Zero;
            }
            _disposed = true;
        }
    }

    ~BrimstoneEngine()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
