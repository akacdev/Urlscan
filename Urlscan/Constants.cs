using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Urlscan
{
    internal class Constants
    {
        /// <summary>
        /// The API version to use when communicating.
        /// </summary>
        public const int Version = 1;

        public static readonly Version HttpVersion = new(2, 0);
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

        public const string UserAgent = "Urlscan C# Client - actually-akac/Urlscan";
        public const string JsonContentType = "application/json";
        public const string HtmlContentType = "text/html";
        public const int ErrorStatusCode = 500;

        public const string SuccessSubmissionMessage = "Submission successful";

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