namespace Brimstone.Net;

/// <summary>
/// Result codes for Brimstone operations
/// </summary>
public enum BsResultCode
{
    Success = 0,
    Error = 1,
    NullPointer = 2,
    InvalidUtf8 = 3,
    PanicCaught = 4,
}
