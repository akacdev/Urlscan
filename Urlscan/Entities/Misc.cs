using System.Text.Json.Serialization;

namespace Urlscan
{
    public enum Visibility
    {
        Public,
        Unlisted,
        Private
    }

    public enum ScanCountry
    {
        Auto,
        DE,
        US,
        JP,
        FR,
        GB,
        NL,
        CA,
        IT,
        ES,
        SE,
        FI,
        DK,
        NO
    }

    public enum LimitType
    {
        Private,
        Public,
        Retrieve,
        Search,
        Unlisted,
        Livescan,
        Liveshot
    }

    public enum VerdictScope
    {
        PageUrl,
        PageDomain,
        TaskUrl,
        TaskDomain
    }

    public enum ThreatType
    {
        Malware,
        UnwantedSoftware,
        PotentiallyHarmfulApplication,
        SocialEngineering,
        BrandImpersonation,
        Phishing,
        Spearphishing,
        Waterholing,
        TechSupportScam,
        Scam,
        Counterfeit,
        Defaced,
        Skimmer,
        Controlpanel,
        Phishkit,
        FakeDocument,
        Misc
    }

    public enum VerdictType
    {
        Malicious,
        Suspicious,
        Legitimate,
        Comment
    }

    public class UrlscanError
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }
    }

    public class Stats
    {
        [JsonPropertyName("running")]
        public int Running { get; set; }

        [JsonPropertyName("queued")]
        public int Queued { get; set; }

        [JsonPropertyName("public")]
        public int Public { get; set; }

        [JsonPropertyName("unlisted")]
        public int Unlisted { get; set; }

        [JsonPropertyName("private")]
        public int Private { get; set; }

        public int Total
        {
            get
            {
                return Running + Public + Unlisted;
            }
        }
    }
}