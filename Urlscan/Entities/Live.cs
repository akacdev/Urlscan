using System;
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
        public LiveScanTask Task { get; set; }

        [JsonPropertyName("stats")]
        public LiveScanStats Stats { get; set; }

        [JsonPropertyName("page")]
        public Page Page { get; set; }

        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("screenshot")]
        public string Screenshot { get; set; }
    }

    public class LiveScanTask
    {
        [JsonPropertyName("visibility")]
        public string Visibility { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("apexDomain")]
        public string ApexDomain { get; set; }

        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("uuid")]
        public string UUID { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }
    }

    public class LiveScanStats
    {
        [JsonPropertyName("uniqIPs")]
        public int UniqIPs { get; set; }

        [JsonPropertyName("uniqCountries")]
        public int UniqCountries { get; set; }

        [JsonPropertyName("dataLength")]
        public int DataLength { get; set; }

        [JsonPropertyName("encodedDataLength")]
        public int EncodedDataLength { get; set; }

        [JsonPropertyName("requests")]
        public int Requests { get; set; }
    }

    public class LiveScanPage
    {
        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("server")]
        public string Server { get; set; }

        [JsonPropertyName("ip")]
        public string Ip { get; set; }

        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("tlsValidDays")]
        public int TlsValidDays { get; set; }

        [JsonPropertyName("tlsAgeDays")]
        public int TlsAgeDays { get; set; }

        [JsonPropertyName("ptr")]
        public string Ptr { get; set; }

        [JsonPropertyName("tlsValidFrom")]
        public DateTime TlsValidFrom { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("umbrellaRank")]
        public int UmbrellaRank { get; set; }

        [JsonPropertyName("apexDomain")]
        public string ApexDomain { get; set; }

        [JsonPropertyName("asnname")]
        public string Asnname { get; set; }

        [JsonPropertyName("asn")]
        public string Asn { get; set; }

        [JsonPropertyName("tlsIssuer")]
        public string TlsIssuer { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("redirected")]
        public string Redirected { get; set; }
    }
}
