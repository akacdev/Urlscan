using System.Text.Json.Serialization;

namespace Urlscan
{
    public class ScanPayload
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("customagent")]
        public string UserAgent { get; set; }

        [JsonPropertyName("referer")]
        public string Referer { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }

        [JsonPropertyName("overrideSafety")]
        public bool? OverrideSafety { get; set; }

        [JsonPropertyName("country")]
        public ScanCountry? Country { get; set; }

        [JsonPropertyName("visibility")]
        public Visibility Visibility { get; set; }
    }

    public class Submission
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("uuid")]
        public string UUID { get; set; }

        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("api")]
        public string Api { get; set; }

        [JsonPropertyName("visibility")]
        public Visibility Visibility { get; set; }

        [JsonPropertyName("options")]
        public Options Options { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }
    }

    public class Options
    {
        [JsonPropertyName("useragent")]
        public string UserAgent { get; set; }
    }
}
