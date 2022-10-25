using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Urlscan
{
    public class Result
    {
        [JsonPropertyName("uuid")]
        public string UUID
        {
            get
            {
                return Task.UUID;
            }
        }

        [JsonPropertyName("task")]
        public ScanTask Task { get; set; }

        [JsonPropertyName("page")]
        public BasicScanPage Page { get; set; }

        [JsonPropertyName("meta")]
        public ScanMeta Meta { get; set; }

        [JsonPropertyName("lists")]
        public ScanLists Lists { get; set; }

        [JsonPropertyName("stats")]
        public ScanStats Stats { get; set; }

        [JsonPropertyName("data")]
        public ScanData Data { get; set; }

        [JsonPropertyName("verdicts")]
        public ScanVerdicts Verdicts { get; set; }

        [JsonPropertyName("submitter")]
        public ScanSubmitter Submitter { get; set; }
    }

    public class ScanTask
    {
        [JsonPropertyName("uuid")]
        public string UUID { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("apexDomain")]
        public string ApexDomain { get; set; }

        [JsonPropertyName("visibility")]
        public Visibility Visibility { get; set; }

        [JsonPropertyName("method")]
        public SubmissionMethod Method { get; set; }

        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }

        /// <summary>
        /// Only available when directly fetching a result.
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("reportURL")]
        public string ReportUrl { get; set; }

        [JsonPropertyName("screenshotURL")]
        public string ScreenshotUrl { get; set; }

        [JsonPropertyName("domURL")]
        public string DOMUrl { get; set; }
    }

    public class BasicScanPage
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("server")]
        public string Server { get; set; }

        [JsonPropertyName("ip")]
        public string IP { get; set; }

        [JsonPropertyName("asn")]
        public string ASN { get; set; }

        [JsonPropertyName("asnname")]
        public string ASNName { get; set; }
    }

    public class ScanPage
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("ip")]
        public string IP { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("server")]
        public string Server { get; set; }

        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; }

        [JsonPropertyName("tlsIssuer")]
        public string TLSIssuer { get; set; }

        [JsonPropertyName("tlsValidDays")]
        public int TLSValidDays { get; set; }

        [JsonPropertyName("tlsAgeDays")]
        public int TLSAgeDays { get; set; }

        [JsonPropertyName("tlsValidFrom")]
        public DateTime TLSValidFrom { get; set; }

        [JsonPropertyName("ptr")]
        public string PTR { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("umbrellaRank")]
        public int UmbrellaRank { get; set; }

        [JsonPropertyName("apexDomain")]
        public string ApexDomain { get; set; }

        [JsonPropertyName("asnname")]
        public string ASNName { get; set; }

        [JsonPropertyName("asn")]
        public string ASN { get; set; }

        [JsonConverter(typeof(StringIntConverter))]
        [JsonPropertyName("status")]
        public int? Status { get; set; }
    }

    public class ScanLists
    {
        [JsonPropertyName("ips")]
        public string[] IPs { get; set; }

        [JsonPropertyName("countries")]
        public string[] Countries { get; set; }

        [JsonPropertyName("asns")]
        public string[] ASNs { get; set; }

        [JsonPropertyName("domains")]
        public string[] Domains { get; set; }

        [JsonPropertyName("servers")]
        public string[] Servers { get; set; }

        [JsonPropertyName("urls")]
        public string[] Urls { get; set; }

        [JsonPropertyName("linkDomains")]
        public string[] LinkDomains { get; set; }

        [JsonPropertyName("certificates")]
        public Certificate[] Certificates { get; set; }

        [JsonPropertyName("hashes")]
        public string[] Hashes { get; set; }
    }

    public class Certificate
    {
        [JsonPropertyName("subjectName")]
        public string SubjectName { get; set; }

        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonConverter(typeof(LongUnixConverter))]
        [JsonPropertyName("validFrom")]
        public DateTime ValidFrom { get; set; }

        [JsonConverter(typeof(LongUnixConverter))]
        [JsonPropertyName("validTo")]
        public DateTime ValidTo { get; set; }
    }

    public class ScanMeta
    {
        [JsonPropertyName("processors")]
        public Processors Processors { get; set; }
    }

    public class Processors
    {
        [JsonPropertyName("umbrella")]
        public UmbrellaProcessor Umbrella { get; set; }

        [JsonPropertyName("geoip")]
        public GeoIPProcessor GeoIP { get; set; }

        [JsonPropertyName("asn")]
        public ASNProcessor ASN { get; set; }

        [JsonPropertyName("wappa")]
        public WappaProcessor Wappa { get; set; }
    }

    public class UmbrellaProcessor
    {
        [JsonPropertyName("data")]
        public UmbrellaProcessorData[] Data { get; set; }
    }

    public class UmbrellaProcessorData
    {
        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("rank")]
        public int Rank { get; set; }
    }

    public class GeoIPProcessor
    {
        [JsonPropertyName("data")]
        public GeoIP[] Data { get; set; }
    }

    public class GeoIPData
    {
        [JsonPropertyName("ip")]
        public string IP { get; set; }

        [JsonPropertyName("geoip")]
        public GeoIP GeoIP { get; set; }
    }

    public class GeoIP
    {
        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("ll")]
        public double[] LL { get; set; }

        [JsonPropertyName("country_name")]
        public string CountryName { get; set; }

        [JsonPropertyName("metro")]
        public int Metro { get; set; }
    }

    public class ASNProcessor
    {
        [JsonPropertyName("data")]
        public ASN[] Data { get; set; }
    }

    public class ASN
    {
        [JsonPropertyName("asn")]
        public string Value { get; set; }

        [JsonPropertyName("ip")]
        public string IP { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("registrar")]
        public string Registrar { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("route")]
        public string Route { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class WappaProcessor
    {
        [JsonPropertyName("data")]
        public WappaProcessorData[] Data { get; set; }
    }

    public class WappaProcessorData
    {
        [JsonPropertyName("confidence")]
        public WappaConfidence[] Confidence { get; set; }

        [JsonPropertyName("confidenceTotal")]
        public int ConfidenceTotal { get; set; }

        [JsonPropertyName("app")]
        public string App { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("website")]
        public string Website { get; set; }

        [JsonPropertyName("categories")]
        public WappaCategory[] Categories { get; set; }
    }

    public class WappaCategory
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("priority")]
        public int Priority { get; set; }
    }

    public class WappaConfidence
    {
        [JsonPropertyName("confidence")]
        public int Confidence { get; set; }

        [JsonPropertyName("pattern")]
        public string Pattern { get; set; }
    }

    public class BasicScanStats
    {
        [JsonPropertyName("uniqIPs")]
        public int UniqueIPCount { get; set; }

        [JsonPropertyName("uniqCountries")]
        public int UniqueCountryCount { get; set; }

        [JsonPropertyName("requests")]
        public int RequestCount { get; set; }

        [JsonPropertyName("dataLength")]
        public int DataLength { get; set; }

        [JsonPropertyName("encodedDataLength")]
        public int EncodedDataLength { get; set; }
    }

    public class ScanStats
    {
        [JsonPropertyName("resourceStats")]
        public ResourceStats[] Resource { get; set; }

        [JsonPropertyName("protocolStats")]
        public ProtocolStats[] Protocol { get; set; }

        [JsonPropertyName("tlsStats")]
        public TLSStats[] TLSStats { get; set; }

        [JsonPropertyName("serverStats")]
        public ServerStats[] Server { get; set; }

        [JsonPropertyName("domainStats")]
        public DomainStats[] Domain { get; set; }

        [JsonPropertyName("regDomainStats")]
        public RegDomainStats[] RegDomain { get; set; }

        [JsonPropertyName("secureRequests")]
        public int SecureRequests { get; set; }

        [JsonPropertyName("securePercentage")]
        public int? SecurePercentage { get; set; }

        [JsonPropertyName("IPv6Percentage")]
        public int? IPv6Percentage { get; set; }

        [JsonPropertyName("uniqCountries")]
        public int UniqueCountryCount { get; set; }

        [JsonPropertyName("totalLinks")]
        public int TotalLinks { get; set; }

        [JsonPropertyName("malicious")]
        public int Malicious { get; set; }

        [JsonPropertyName("adBlocked")]
        public int AdBlocked { get; set; }
    }

    public class ResourceStats
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("encodedSize")]
        public int EncodedSize { get; set; }

        [JsonPropertyName("latency")]
        public int Latency { get; set; }

        [JsonPropertyName("countries")]
        public string[] Countries { get; set; }

        [JsonPropertyName("ips")]
        public string[] IPs { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("compression")]
        public string Compression { get; set; }

        [JsonPropertyName("percentage")]
        public int Percentage { get; set; }
    }

    public class ProtocolStats
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("encodedSize")]
        public int EncodedSize { get; set; }

        [JsonPropertyName("ips")]
        public string[] IPs { get; set; }

        [JsonPropertyName("countries")]
        public string[] Countries { get; set; }

        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }
    }

    public class TLSStats
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("encodedSize")]
        public int EncodedSize { get; set; }

        [JsonPropertyName("ips")]
        public string[] IPs { get; set; }

        [JsonPropertyName("countries")]
        public string[] Countries { get; set; }

        [JsonPropertyName("protocols")]
        public Dictionary<string, int> Protocols { get; set; }

        [JsonPropertyName("securityState")]
        public string SecurityState { get; set; }
    }

    public class ServerStats
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("encodedSize")]
        public int EncodedSize { get; set; }

        [JsonPropertyName("ips")]
        public string[] IPs { get; set; }

        [JsonPropertyName("countries")]
        public string[] Countries { get; set; }

        [JsonPropertyName("server")]
        public string Server { get; set; }
    }

    public class DomainStats
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("ips")]
        public string[] IPs { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("encodedSize")]
        public int EncodedSize { get; set; }

        [JsonPropertyName("countries")]
        public string[] Countries { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("initiators")]
        public string[] Initiators { get; set; }

        [JsonPropertyName("redirects")]
        public int? Redirects { get; set; }
    }

    public class RegDomainStats
    {
        [JsonPropertyName("count")]
        public int? Count { get; set; }

        [JsonPropertyName("ips")]
        public string[] IPs { get; set; }

        [JsonPropertyName("regDomain")]
        public string RegDomain { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("encodedSize")]
        public int? EncodedSize { get; set; }

        [JsonPropertyName("countries")]
        public string[] Countries { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("subDomains")]
        public SubDomainStats[] SubDomains { get; set; }

        [JsonPropertyName("redirects")]
        public int? Redirects { get; set; }
    }

    public class SubDomainStats
    {
        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("failed")]
        public bool? Failed { get; set; }
    }

    public class IPStats
    {
        [JsonPropertyName("requests")]
        public int Requests { get; set; }

        [JsonPropertyName("domains")]
        public string[] Domains { get; set; }

        [JsonPropertyName("ip")]
        public string IP { get; set; }

        [JsonPropertyName("asn")]
        public ASN ASN { get; set; }

        [JsonPropertyName("geoip")]
        public GeoIP GeoIP { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("encodedSize")]
        public int EncodedSize { get; set; }

        [JsonPropertyName("countries")]
        public string[] Countries { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("ipv6")]
        public bool IPv6 { get; set; }

        [JsonPropertyName("redirects")]
        public int Redirects { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class ScanData
    {
        [JsonPropertyName("requests")]
        public DataRequest[] Requests { get; set; }

        [JsonPropertyName("cookies")]
        public object Items
        {
            get
            {
                return _items;
            }

            set
            {
                if (((JsonElement)value).ValueKind.ToString() == "Array")
                {
                    _items = ((JsonElement)value).Deserialize<DataCookie[]>();
                }
            }
        }

        [JsonIgnore]
        private DataCookie[] _items;

        [JsonPropertyName("console")]
        public DataConsole[] Console { get; set; }

        [JsonPropertyName("links")]
        public DataLink[] Links { get; set; }

        [JsonPropertyName("timing")]
        public DataTiming Timing { get; set; }

        [JsonPropertyName("globals")]
        public DataGlobal[] Globals { get; set; }
    }

    public class DataCookie
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("expires")]
        public double Expires { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("httpOnly")]
        public bool HttpOnly { get; set; }

        [JsonPropertyName("secure")]
        public bool Secure { get; set; }

        [JsonPropertyName("session")]
        public bool Session { get; set; }

        [JsonPropertyName("sameSite")]
        public string SameSite { get; set; }

        [JsonPropertyName("priority")]
        public string Priority { get; set; }

        [JsonPropertyName("sameParty")]
        public bool SameParty { get; set; }

        [JsonPropertyName("sourceScheme")]
        public string SourceScheme { get; set; }

        [JsonPropertyName("sourcePort")]
        public int SourcePort { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("level")]
        public string Level { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("timestamp")]
        public double Timestamp { get; set; }

        [JsonPropertyName("line")]
        public int? Line { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class DataConsole
    {
        [JsonPropertyName("message")]
        public Message Message { get; set; }
    }

    public class DataLink
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class DataTiming
    {
        [JsonPropertyName("beginNavigation")]
        public DateTime BeginNavigation { get; set; }

        [JsonPropertyName("frameStartedLoading")]
        public DateTime FrameStartedLoading { get; set; }

        [JsonPropertyName("frameNavigated")]
        public DateTime FrameNavigated { get; set; }

        [JsonPropertyName("domContentEventFired")]
        public DateTime DomContentEventFired { get; set; }

        [JsonPropertyName("frameStoppedLoading")]
        public DateTime FrameStoppedLoading { get; set; }
    }

    public class DataGlobal
    {
        [JsonPropertyName("prop")]
        public string Prop { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class DataRequest
    {
        [JsonPropertyName("request")]
        public Request Request { get; set; }

        [JsonPropertyName("response")]
        public Response Response { get; set; }
    }

    public class Timing
    {
        [JsonPropertyName("requestTime")]
        public double RequestTime { get; set; }

        [JsonPropertyName("proxyStart")]
        public double ProxyStart { get; set; }

        [JsonPropertyName("proxyEnd")]
        public double ProxyEnd { get; set; }

        [JsonPropertyName("dnsStart")]
        public double DNSStart { get; set; }

        [JsonPropertyName("dnsEnd")]
        public double DNSEnd { get; set; }

        [JsonPropertyName("connectStart")]
        public double ConnectStart { get; set; }

        [JsonPropertyName("connectEnd")]
        public double ConnectEnd { get; set; }

        [JsonPropertyName("sslStart")]
        public double SSLStart { get; set; }

        [JsonPropertyName("sslEnd")]
        public double SSLEnd { get; set; }

        [JsonPropertyName("workerStart")]
        public double WorkerStart { get; set; }

        [JsonPropertyName("workerReady")]
        public double WorkerReady { get; set; }

        [JsonPropertyName("workerFetchStart")]
        public double WorkerFetchStart { get; set; }

        [JsonPropertyName("workerRespondWithSettled")]
        public double WorkerRespondWithSettled { get; set; }

        [JsonPropertyName("sendStart")]
        public double SendStart { get; set; }

        [JsonPropertyName("sendEnd")]
        public double SendEnd { get; set; }

        [JsonPropertyName("pushStart")]
        public double PushStart { get; set; }

        [JsonPropertyName("pushEnd")]
        public double PushEnd { get; set; }

        [JsonPropertyName("receiveHeadersEnd")]
        public double ReceiveHeadersEnd { get; set; }
    }

    public class SecurityDetails
    {
        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("keyExchange")]
        public string KeyExchange { get; set; }

        [JsonPropertyName("keyExchangeGroup")]
        public string KeyExchangeGroup { get; set; }

        [JsonPropertyName("cipher")]
        public string Cipher { get; set; }

        [JsonPropertyName("certificateId")]
        public int CertificateId { get; set; }

        [JsonPropertyName("subjectName")]
        public string SubjectName { get; set; }

        [JsonPropertyName("sanList")]
        public string[] SanList { get; set; }

        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonConverter(typeof(LongUnixConverter))]
        [JsonPropertyName("validFrom")]
        public DateTime ValidFrom { get; set; }

        [JsonConverter(typeof(LongUnixConverter))]
        [JsonPropertyName("validTo")]
        public DateTime ValidTo { get; set; }

        [JsonPropertyName("certificateTransparencyCompliance")]
        public string CertificateTransparencyCompliance { get; set; }
    }

    public class SecurityHeader
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class Request
    {
        [JsonPropertyName("request")]
        public InnerRequest InnerRequest { get; set; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }

        [JsonPropertyName("loaderId")]
        public string LoaderId { get; set; }

        [JsonPropertyName("documentURL")]
        public string DocumentURL { get; set; }

        [JsonPropertyName("timestamp")]
        public double Timestamp { get; set; }

        [JsonPropertyName("wallTime")]
        public double WallTime { get; set; }

        [JsonPropertyName("redirectHasExtraInfo")]
        public bool RedirectHasExtraInfo { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("frameId")]
        public string FrameId { get; set; }

        [JsonPropertyName("hasUserGesture")]
        public bool HasUserGesture { get; set; }

        [JsonPropertyName("primaryRequest")]
        public bool PrimaryRequest { get; set; }
    }

    public class InnerRequest
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("headers")]
        public Dictionary<string, string> Headers { get; set; }

        [JsonPropertyName("mixedContentType")]
        public string MixedContentType { get; set; }

        [JsonPropertyName("initialPriority")]
        public string InitialPriority { get; set; }

        [JsonPropertyName("referrerPolicy")]
        public string ReferrerPolicy { get; set; }

        [JsonPropertyName("isSameSite")]
        public bool IsSameSite { get; set; }
    }

    public class Response
    {
        [JsonPropertyName("response")]
        public InnerResponse InnerResponse { get; set; }

        [JsonPropertyName("encodedDataLength")]
        public int EncodedDataLength { get; set; }

        [JsonPropertyName("dataLength")]
        public int DataLength { get; set; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("hasExtraInfo")]
        public bool HasExtraInfo { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("asn")]
        public ASN ASN { get; set; }

        [JsonPropertyName("geoip")]
        public GeoIP GeoIP { get; set; }
    }

    public class InnerResponse
    {
        [JsonPropertyName("url")]
        public string URL { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("statusText")]
        public string StatusText { get; set; }

        [JsonPropertyName("headers")]
        public Dictionary<string, string> Headers { get; set; }

        [JsonPropertyName("mimeType")]
        public string MIMEType { get; set; }

        [JsonPropertyName("remoteIPAddress")]
        public string RemoteIPAddress { get; set; }

        [JsonPropertyName("remotePort")]
        public int RemotePort { get; set; }

        [JsonPropertyName("encodedDataLength")]
        public int EncodedDataLength { get; set; }

        [JsonPropertyName("timing")]
        public Timing Timing { get; set; }

        [JsonPropertyName("responseTime")]
        public double ResponseTime { get; set; }

        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("securityState")]
        public string SecurityState { get; set; }

        [JsonPropertyName("securityDetails")]
        public SecurityDetails SecurityDetails { get; set; }

        [JsonPropertyName("securityHeaders")]
        public SecurityHeader[] SecurityHeaders { get; set; }
    }

    public class ScanVerdicts
    {
        [JsonPropertyName("overall")]
        public OverallVerdict Overall { get; set; }

        [JsonPropertyName("urlscan")]
        public UrlscanVerdict Urlscan { get; set; }

        [JsonPropertyName("community")]
        public CommunityVerdict Community { get; set; }
    }

    public class OverallVerdict
    {
        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("categories")]
        public string[] Categories { get; set; }

        [JsonPropertyName("brands")]
        public string[] Brands { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }

        [JsonPropertyName("malicious")]
        public bool Malicious { get; set; }

        [JsonPropertyName("hasVerdicts")]
        public bool HasVerdicts { get; set; }

        [JsonPropertyName("lastVerdict")]
        public DateTime? LastVerdict { get; set; }
    }

    public class UrlscanVerdict
    {
        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("categories")]
        public string[] Categories { get; set; }

        [JsonPropertyName("brands")]
        public Brand[] Brands { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }

        [JsonPropertyName("malicious")]
        public bool Malicious { get; set; }

        [JsonPropertyName("hasVerdicts")]
        public bool HasVerdicts { get; set; }
    }

    public class CommunityVerdict
    {
        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("categories")]
        public string[] Categories { get; set; }

        [JsonPropertyName("brands")]
        public Brand[] Brands { get; set; }

        [JsonPropertyName("votesTotal")]
        public int VotesTotal { get; set; }

        [JsonPropertyName("votesMalicious")]
        public int VotesMalicious { get; set; }

        [JsonPropertyName("votesBenign")]
        public int VotesBenign { get; set; }

        [JsonPropertyName("malicious")]
        public bool Malicious { get; set; }

        [JsonPropertyName("hasVerdicts")]
        public bool HasVerdicts { get; set; }

        [JsonPropertyName("lastVerdict")]
        public DateTime? LastVerdict { get; set; }
    }

    public class Brand
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("country")]
        public string[] Country { get; set; }

        [JsonPropertyName("vertical")]
        public string[] Vertical { get; set; }
    }

    public class ScanSubmitter
    {
        [JsonPropertyName("country")]
        public string Country { get; set; }
    }
}