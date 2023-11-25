using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Urlscan
{
    internal class Constants
    {
        /// <summary>
        /// The version of the API to send requests to.
        /// </summary>
        public const int Version = 1;
        /// <summary>
        /// The base URI to send requests to.
        /// </summary>
        public static readonly Uri BaseUri = new("https://urlscan.io/");
        /// <summary>
        /// The preferred HTTP request version to use.
        /// </summary>
        public static readonly Version HttpVersion = new(2, 0);
        /// <summary>
        /// The <c>User-Agent</c> header value to send along requests.
        /// </summary>
        public const string UserAgent = "Urlscan C# Client - actually-akac/Urlscan";
        /// <summary>
        /// The <c>User-Agent</c> header value to send along requests when using <see cref="LiveClient"/>.
        /// </summary>
        public const string LiveUserAgent = "Urlscan C# Live Client - actually-akac/Urlscan";
        /// <summary>
        /// Up to how many times should submitted scans get polled for completion before erroring.
        /// </summary>
        public const int MaxPollingRetries = 20;
        /// <summary>
        /// A string to identify a successfull submission. Used to confirm that a scan was successfully submitted.
        /// </summary>
        public const string SuccessSubmissionMessage = "Submission successful";
        /// <summary>
        /// A string to identify that zero similar hits were detected for a scan.
        /// </summary>
        public const string ZeroSimilarHitsMessage = "0 structurally similar hits on different domains, IPs and ASNs";
        /// <summary>
        /// The maximum string length when displaying a preview of a response body.
        /// </summary>
        public const int PreviewMaxLength = 500;

        /// <summary>
        /// The default JSON options object with a lower case naming policy.
        /// </summary>
        public static readonly JsonSerializerOptions JsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(new LowerCaseNamingPolicy())
            }
        };

        /// <summary>
        /// A specialized JSON options object for submitting verdicts.
        /// </summary>
        public static readonly JsonSerializerOptions VerdictJsonOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter(new VerdictNamingPolicy())
            }
        };
    }
}