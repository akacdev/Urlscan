using System.Text.Json;
using System.Text.Json.Serialization;

namespace Urlscan
{
    public class Constants
    {
        public const string JSONContentType = "application/json";
        public static readonly string HTMLContentType = "text/html";
        public const int ErrorStatusCode = 500;
        public const string SuccessSubmissionMessage = "Submission successful";

        public static readonly JsonSerializerOptions JsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(new EnumNamingPolicy())
            }
        };

        public static readonly JsonSerializerOptions VerdictJsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter(new VerdictNamingPolicy())
            }
        };
    }
}