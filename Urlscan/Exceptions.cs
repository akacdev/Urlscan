using System;

namespace Urlscan
{
    /// <summary>
    /// A custom Urlscan exception.
    /// </summary>
    public class UrlscanException : Exception
    {
        /// <summary>
        /// Milliseconds until you can retry this request again. Provided when there is a ratelimit.
        /// </summary>
        public int? RetryAfter { get; set; }

        /// <summary>
        /// An user-friendly Urlscan error.
        /// </summary>
        public UrlscanError Error { get; set; }

        public UrlscanException(string message) : base(message) { }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() :
            base($"This request requires a valid session cookie. You aren't logged in using your secure identifier cookie. For a guide on obtaining this value, please see the GitHub repository.")
        { }
    }
}