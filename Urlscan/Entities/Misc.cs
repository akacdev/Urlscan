using System.Text.Json.Serialization;

namespace Urlscan
{
    /// <summary>
    /// Determines the visibility of a scan.<br/>
    /// Official docs: <a href="https://urlscan.io/blog/2022/07/27/scan-visibility-best-practices/"></a>
    /// </summary>
    public enum Visibility
    {
        /// <summary>
        /// <b>Public</b> scans are visible by anyone, including guest users and search engines.
        /// </summary>
        Public,
        /// <summary>
        /// <b>Unlisted</b> scans are only visible to customers of the Urlscan Pro platform. 
        /// </summary>
        Unlisted,
        /// <summary>
        /// <b>Private</b> scans are only visible to you, and possibly the members of your team.
        /// </summary>
        Private
    }

    /// <summary>
    /// Determines which country the scan is performed in. Only available in Public/Unlisted scans in the free subscription.
    /// </summary>
    public enum ScanCountry
    {
        /// <summary>
        /// Use <b>Auto</b> whenever you don't care about the country of the scan.
        /// </summary>
        Auto,
        /// <summary>
        /// Scan from <b>Germany</b>.
        /// </summary>
        DE,
        /// <summary>
        /// Scan from the <b>United States</b>.
        /// </summary>
        US,
        /// <summary>
        /// Scan from <b>Japan</b>.
        /// </summary>
        JP,
        /// <summary>
        /// Scan from <b>France</b>.
        /// </summary>
        FR,
        /// <summary>
        /// Scan from the <b>Great Britain</b>.
        /// </summary>
        GB,
        /// <summary>
        /// Scan from the <b>Netherlands</b>.
        /// </summary>
        NL,
        /// <summary>
        /// Scan from <b>Canada</b>.
        /// </summary>
        CA,
        /// <summary>
        /// Scan from <b>Italy</b>.
        /// </summary>
        IT,
        /// <summary>
        /// Scan from <b>Spain</b>.
        /// </summary>
        ES,
        /// <summary>
        /// Scan from <b>Sweden</b>.
        /// </summary>
        SE,
        /// <summary>
        /// Scan from <b>Finland</b>.
        /// </summary>
        FI,
        /// <summary>
        /// Scan from <b>Denmark</b>.
        /// </summary>
        DK,
        /// <summary>
        /// Scan from <b>Norway</b>.
        /// </summary>
        NO,
        /// <summary>
        /// Scan from <b>Iceland</b>.
        /// </summary>
        IS,
        /// <summary>
        /// Scan from <b>Australia</b>.
        /// </summary>
        AU
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

    /// <summary>
    /// Determines which sections a verdict applies to.
    /// </summary>
    public enum VerdictScope
    {
        PageUrl,
        PageDomain,
        TaskUrl,
        TaskDomain
    }

    /// <summary>
    /// Determines which threat types a verdict applies to.
    /// </summary>
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

    /// <summary>
    /// Determines the type of a verdict.
    /// </summary>
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