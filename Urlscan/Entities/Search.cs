using System.Text.Json;
using System.Text.Json.Serialization;

namespace Urlscan
{
    public class SearchContainer
    {
        [JsonPropertyName("results")]
        public SearchItem[] Results { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("took")]
        public int Took { get; set; }

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }

    public class SearchItem
    {
        [JsonPropertyName("task")]
        public ScanTask Task { get; set; }

        [JsonPropertyName("stats")]
        public BasicScanStats Stats { get; set; }

        [JsonPropertyName("page")]
        public ScanPage Page { get; set; }

        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("sort")]
        public JsonElement[] Sort { get; set; }

        [JsonPropertyName("result")]
        public string ResultUrl { get; set; }

        [JsonPropertyName("screenshot")]
        public string ScreenshotUrl { get; set; }
    }
}