using System.Text.Json.Serialization;

namespace Urlscan
{
    public class LiveContainer
    {
        [JsonPropertyName("results")]
        public LiveScan[] Results { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("took")]
        public int Took { get; set; }

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }

    public class LiveScan
    {
        [JsonPropertyName("task")]
        public ScanTask Task { get; set; }

        [JsonPropertyName("stats")]
        public BasicScanStats Stats { get; set; }

        [JsonPropertyName("page")]
        public ScanPage Page { get; set; }

        [JsonPropertyName("result")]
        public string ResultUrl { get; set; }

        [JsonPropertyName("screenshot")]
        public string ScreenshotUrl { get; set; }
    }
}