namespace Brimstone.Net;

/// <summary>
/// Exception thrown when a Brimstone operation fails
/// </summary>
public class BrimstoneException : Exception
{
    public BsResultCode ResultCode { get; }

    public BrimstoneException(string message, BsResultCode resultCode)
        : base(message)
    {
        ResultCode = resultCode;
    }

    public BrimstoneException(string message, BsResultCode resultCode, Exception innerException)
        : base(message, innerException)
    {
        ResultCode = resultCode;
    }
}
