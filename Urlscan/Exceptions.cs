using System;

namespace Urlscan
{
    public class NxDomainException : Exception { public NxDomainException(string message) : base(message) { } }
    public class SillyException : Exception { public SillyException(string message) : base(message) { } }
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() :
            base($"This request requires a user's cookie. You aren't logged in using your secure identifier cookie. For a guide of obtaining this value, please see the GitHub.")
        { }
    }
}
