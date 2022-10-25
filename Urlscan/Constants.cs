using System.Text.Json;
using System.Text.Json.Serialization;

namespace Urlscan
{
    internal class Constants
    {
        public const string JsonContentType = "application/json";
        public static readonly string HtmlContentType = "text/html";
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