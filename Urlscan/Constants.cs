using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Urlscan
{
    internal class Constants
    {
        public const int Version = 1;
        public static readonly Uri BaseUri = new("https://urlscan.io/");
        public static readonly Version HttpVersion = new(2, 0);

        public const string UserAgent = "Urlscan C# Client - actually-akac/Urlscan";
        public const string LiveUserAgent = "Urlscan C# Live Client - actually-akac/Urlscan";

        public const string AcceptedEncoding = "gzip, deflate, br";
        public const string JsonContentType = "application/json";
        public const string HtmlContentType = "text/html";

        public const int ErrorStatusCode = 500;
        public const int MaxPollingRetries = 20;

        public const string SuccessSubmissionMessage = "Submission successful";
        public const string ZeroSimilarHitsMessage = "0 structurally similar hits on different domains, IPs and ASNs";

        public static readonly JsonSerializerOptions JsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(new LowerCaseNamingPolicy())
            }
        };

        public static readonly JsonSerializerOptions VerdictJsonOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter(new VerdictNamingPolicy())
            }
        };
    }
}