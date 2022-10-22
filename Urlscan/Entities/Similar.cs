using System.Text.Json.Serialization;

namespace Urlscan
{
    public class SimilarScan
    {
        [JsonPropertyName("uuid")]
        public string UUID { get; set; }

        [JsonPropertyName("url")]
        public string URL { get; set; }
    }
}
