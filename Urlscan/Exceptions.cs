using System;

namespace Urlscan
{
    public class UrlscanException : Exception { public UrlscanException(string message) : base(message) { } }
    public class NXDOMAINException : Exception { public NXDOMAINException(string message) : base(message) { } }
    public class SillyException : Exception { public SillyException(string message) : base(message) { } }
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() :
            base($"This request requires a valid session cookie. You aren't logged in using your secure identifier cookie. For a guide on obtaining this value, please see the GitHub repository.")
        { }
    }
}