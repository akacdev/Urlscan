using System.Text.Json.Serialization;

namespace Urlscan
{
    public class ScanParameters
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }

        [JsonPropertyName("customagent")]
        public string UserAgent { get; set; }

        [JsonPropertyName("referer")]
        public string Referer { get; set; }

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

        [JsonPropertyName("api")]
        public string API { get; set; }

        [JsonPropertyName("uuid")]
        public string UUID { get; set; }

        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("visibility")]
        public Visibility Visibility { get; set; }

        [JsonPropertyName("options")]
        public SubmissionOptions Options { get; set; }

        [JsonPropertyName("url")]
        public string URL { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }
    }

    public class SubmissionOptions
    {
        [JsonPropertyName("useragent")]
        public string UserAgent { get; set; }

        [JsonPropertyName("headers")]
        public HeaderOptions HeaderOptions { get; set; }
    }

    public class HeaderOptions
    {
        [JsonPropertyName("useragent")]
        public string Referer { get; set; }
    }
}