using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Urlscan
{
    /// <summary>
    /// The main class for interacting with the Urlscan API. 
    /// </summary>
    public class UrlscanClient
    {
        /// <summary>
        /// The base API url.
        /// </summary>
        private const string URL = "https://urlscan.io";
        /// <summary>
        /// The API version to use. This was always 1. 
        /// </summary>
        private const int Version = 1;
        /// <summary>
        /// How many times result polling should be retried.
        /// </summary>
        private const int MaxRetries = 10;

        private readonly HttpClientHandler HttpHandler = new()
        {
            AutomaticDecompression = DecompressionMethods.All
        };

        private readonly HttpClient Client;
        private readonly JsonSerializerOptions JsonOptions = new();
        private readonly JsonSerializerOptions VerdictJsonOptions = new();

        private readonly string _key;
        private readonly string _sid;
        public readonly bool UsesAccountSID;

        /// <summary>
        /// Create a new instance of the client for interacting with the main API.
        /// </summary>
        /// <param name="key">Your Urlscan API key. See the GitHub for instructions on obtaing one.</param>
        /// <param name="sid">(<b>Optional</b>) Your account's secure identifier cookie for performing operations not implemented in the official API.<para>See the GitHub for instructions on obtaing one.</para></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UrlscanClient(string key, string sid = null)
        {
            _key = key; if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key), "An empty or null API Key was provided.");
            _sid = sid;
            UsesAccountSID = sid is not null;

            Client = new(HttpHandler);

            Client.DefaultRequestVersion = new Version(2, 0);
            Client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Urlscan C# Client - actually-akac/Urlscan");
            Client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            Client.DefaultRequestHeaders.Add("API-Key", _key);

            JsonOptions.Converters.Add(new JsonStringEnumConverter(new EnumNamingPolicy()));
            VerdictJsonOptions.Converters.Add(new JsonStringEnumConverter(new VerdictNamingPolicy()));
        }

        /// <summary>
        /// Fetcht the Urlscan statistics, such as how many scans are currently running.
        /// </summary>
        public async Task<Stats> Stats()
        {
            HttpResponseMessage res = await Client.Request($"{URL}/stats", HttpMethod.Get);

            return await res.Deseralize<Stats>();
        }

        /// <summary>
        /// Submit a new scan of an URL.
        /// </summary>
        /// <param name="url">The URL to scan.</param>
        /// <param name="tags">The custom tags you want to attach to your scan.</param>
        /// <param name="userAgent">The custom User Agent to use in the scan.</param>
        /// <param name="referer">The custom Referer to use in the scan.</param>
        /// <param name="overrideSafety">When true, checking for PII (<b>P</b>ersonally <b>I</b>dentifiable <b>I</b>nformation) will be suppressed. This is dangerous.</param>
        /// <param name="visibility">The visibility of your scan. Defaults to <see cref="Visibility.Public"/></param>
        /// <param name="country">The country to use within your scan.</param>
        /// <returns></returns>
        public async Task<Submission> Scan
        (
            string url,
            string[] tags = null,
            string userAgent = null,
            string referer = null,
            bool overrideSafety = false,
            Visibility visibility = Visibility.Public,
            ScanCountry country = ScanCountry.Auto
        )
        {
            return await Scan(new ScanPayload()
            {
                Url = url,
                UserAgent = userAgent,
                Referer = referer,
                Tags = tags,
                OverrideSafety = overrideSafety,
                Visibility = visibility,
                Country = country
            });
        }

        /// <summary>
        /// Submit a new scan of an URL.
        /// </summary>
        /// <param name="payload">The scan payload object with all the necessary parameters.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Submission> Scan(ScanPayload payload)
        {
            if (payload.Url is null) throw new ArgumentNullException(nameof(payload.Url), "Scan URL is null or empty.");
            if (payload.Country == ScanCountry.Auto) payload.Country = null;
            if (payload.OverrideSafety != true) payload.OverrideSafety = null;

            HttpResponseMessage res = await Client.Request($"{URL}/api/v{Version}/scan", HttpMethod.Post, JsonSerializer.Serialize(payload, JsonOptions));

            return await res.Deseralize<Submission>(JsonOptions);
        }

        /// <summary>
        /// Fetch a result.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns>The result, or <see langword="null"></see> if none is found.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Result> Result(string uuid)
        {
            if (string.IsNullOrEmpty(uuid)) throw new ArgumentNullException(nameof(uuid), "Submission's UUID is null or empty.");

            HttpResponseMessage res = await Client.Request($"{URL}/api/v{Version}/result/{uuid}", HttpMethod.Get, target: HttpStatusCode.OK | HttpStatusCode.NotFound);

            if (res.StatusCode == HttpStatusCode.OK) return await res.Deseralize<Result>();
            else return null;
        }

        /// <summary>
        /// Polls for a submission's result until the scan is finished.
        /// </summary>
        /// <param name="subm">The submission object to send.</param>
        /// <param name="delay">The delay until polling starts.</param>
        /// <param name="interval">The interval between individual polls.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Result> Poll(Submission subm, int delay = 5000, int interval = 2000)
        {
            if (subm is null) throw new ArgumentNullException(nameof(subm), "Submission object is null.");

            return await Poll(subm.UUID, delay, interval);
        }

        /// <summary>
        /// Polls for a submission's result until the scan is finished.
        /// </summary>
        /// <param name="uuid">The UUID of the scan you want to poll for.</param>
        /// <param name="delay">The delay until polling starts.</param>
        /// <param name="interval">The interval between individual polls.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<Result> Poll(string uuid, int delay = 5000, int interval = 2000)
        {
            if (string.IsNullOrEmpty(uuid)) throw new ArgumentNullException(nameof(uuid), "Submission's UUID is null or empty.");
            if (delay <= 0) throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be a positive value.");
            if (delay >= 60000) throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be less than a minute.");
            if (interval <= 0) throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be a positive value.");
            if (interval >= 60000) throw new ArgumentOutOfRangeException(nameof(delay), "Interval must be less than a minute.");

            Result res;
            int retries = 0;

            await Task.Delay(delay);

            while (retries < MaxRetries)
            {
                res = await Result(uuid);

                if (res is not null) return res;
                else await Task.Delay(interval);

                retries++;
            }

            throw new($"Ran out of retry attempts while polling the result of submission {uuid}");
        }

        /// <summary>
        /// Returns basic information about your account.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        public async Task<User> UserInfo()
        {
            if (!UsesAccountSID) throw new UnauthorizedException();

            HttpResponseMessage res = await Client.Request($"{URL}/user/username", HttpMethod.Get, sid: _sid);

            return await res.Deseralize<User>(JsonOptions);
        }

        /// <summary>
        /// Download the screenshot of a scan as a byte array.
        /// </summary>
        /// <param name="res">The result you want to download a screenshot of.</param>
        /// <returns>The screenshot as a byte array, or <see langword="null"></see> if none is found.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<byte[]> DownloadScreenshot(Result res)
        {
            if (res is null) throw new ArgumentNullException(nameof(res), "Result object is null.");

            return await DownloadScreenshot(res.Task.UUID);
        }

        /// <summary>
        /// Download the screenshot of a scan as a byte array.
        /// </summary>
        /// <param name="res">The UUID of a result you want to download a screenshot of.</param>
        /// <returns>The screenshot as a byte array, or <see langword="null"></see> if none is found.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<byte[]> DownloadScreenshot(string uuid)
        {
            if (string.IsNullOrEmpty(uuid)) throw new ArgumentNullException(nameof(uuid), "Result's UUID is null or empty.");

            Stream str = await DownloadScreenshotStream(uuid);
            if (str is null) return null;

            return ((MemoryStream)str).ToArray();
        }

        /// <summary>
        /// Download the screenshot of a scan as a stream.
        /// </summary>
        /// <param name="res">The UUID of a result you want to download a screenshot of.</param>
        /// <returns>The stream, or <see langword="null"></see> if none is found.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Stream> DownloadScreenshotStream(string uuid)
        {
            if (string.IsNullOrEmpty(uuid)) throw new ArgumentNullException(nameof(uuid), "Result's UUID is null or empty.");

            HttpResponseMessage res = await Client.Request($"{URL}/screenshots/{uuid}.png", HttpMethod.Get, target: HttpStatusCode.OK | HttpStatusCode.NotFound);
            if (res.StatusCode == HttpStatusCode.NotFound) return null;

            return res.Content.ReadAsStream();
        }

        /// <summary>
        /// Downlod the DOM/content of a scan as a string.
        /// </summary>
        /// <param name="res">The result you want to download the DOM of.</param>
        /// <returns>The DOM, or <see langword="null"></see> if none is found.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<string> DownloadDOM(Result res)
        {
            if (res is null) throw new ArgumentNullException(nameof(res), "Result object is null.");

            return await DownloadDOM(res.Task.UUID);
        }

        /// <summary>
        /// Downlod the DOM/content of a scan as a string.
        /// </summary>
        /// <param name="uuid">The UUID of a result you want to download the DOM of.</param>
        /// <returns>The DOM, or <see langword="null"></see> if none is found.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<string> DownloadDOM(string uuid)
        {
            if (string.IsNullOrEmpty(uuid)) throw new ArgumentNullException(nameof(uuid), "Result's UUID is null or empty.");

            HttpResponseMessage res = await Client.Request($"{URL}/dom/{uuid}", HttpMethod.Get, target: HttpStatusCode.OK | HttpStatusCode.NotFound);
            if (res.StatusCode == HttpStatusCode.NotFound) return null;
            
            return Encoding.UTF8.GetString(await res.Content.ReadAsByteArrayAsync());
        }

        /// <summary>
        /// Search for past scans using an Elastic Search Query.<para>Docs: <seealso href="https://urlscan.io/docs/search/"/></para>
        /// </summary>
        /// <param name="query">The query to search with.</param>
        /// <param name="amount">The amount of search results you want to retrieve. The limit is <c>10000</c> for free accounts.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="UnauthorizedException"></exception>
        public async Task<SearchResult[]> Search(string query, int amount = 100)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query), "Query is null or empty.");
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be a positive value.");
            if (query.Contains('*') && !UsesAccountSID) throw new UnauthorizedException();

            string searchAfter = null;
            int chunk = (amount > 10000) ? 10000 : amount;

            List<SearchResult> results = new(amount);

            while (results.Count < amount)
            {
                if ((amount - results.Count) < chunk) chunk = amount - results.Count;

                HttpResponseMessage res = await Client.Request(
                    $"{URL}/api/v{Version}/search/?q={query}&size={chunk}{(searchAfter is not null ? $"&search_after={searchAfter}" : "")}",
                    HttpMethod.Get);

                SearchContainer cont = await res.Deseralize<SearchContainer>();
                if (!cont.HasMore || cont.Results.Length == 0) return cont.Results;

                JsonElement[] sort = cont.Results.Last().Sort;

                long sort1 = sort[0].GetInt64();
                string sort2 = sort[1].GetString();

                searchAfter = $"{sort1},{sort2}";

                results.AddRange(cont.Results);
            }

            return results.ToArray();
        }

        /// <summary>
        /// Submit a community verdict to a scan to tell others whether the website is safe or malicious.
        /// </summary>
        /// <param name="result">The result to verdict on.</param>
        /// <param name="scope">The scope that should be verdicted.<para>For example, use <see cref="VerdictScope.TaskDomain"/> when the entire domain is malicious, and <see cref="VerdictScope.TaskUrl"/> if just the specific URL.</para></param>
        /// <param name="type">The type of your verdict.</param>
        /// <param name="comment">A short comment about the scan. Has to be at least <c>40</c> characters long.</param>
        /// <param name="brands">The impersonated brands, like <c>Discord</c> or <c>Steam</c>, if any.</param>
        /// <param name="threats">The threats that are hiding on this website, if any.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task Verdict(Result result, VerdictScope scope, VerdictType type, string comment, string[] brands, ThreatType[] threats)
        {
            string scopeValue = scope switch
            { 
                VerdictScope.PageDomain => result.Page.Domain,
                VerdictScope.PageUrl => result.Page.Url,
                _ => throw new NotImplementedException($"{scope} is an unknown verdict scope.")
            };

            await Verdict(new VerdictPayload()
            {
                UUID = result.Task.UUID,
                Scope = scope,
                Verdict = type,
                ScopeValue = scopeValue,
                Comment = comment,
                Brands = brands,
                ThreatTypes = threats
            });
        }

        /// <summary>
        /// Submit a community verdict to a scan to tell others whether the website is safe or malicious.
        /// </summary>
        /// <param name="scope">The scope that should be verdicted.<para>For example, use <see cref="VerdictScope.TaskDomain"/> when the entire domain is malicious, and <see cref="VerdictScope.TaskUrl"/> if just the specific URL.</para></param>
        /// <param name="scopeValue">The value of the scope you're verdicting. This is either a full URL or a hostname.<para><b>Consider using the method overload that accepts a <c>Result</c> to have this property generated automatically.</b></para></param>
        /// <param name="type">The type of your verdict.</param>
        /// <param name="comment">A short comment about the scan. Has to be at least <c>40</c> characters long.</param>
        /// <param name="brands">The impersonated brands, like <c>Discord</c> or <c>Steam</c>, if any.</param>
        /// <param name="threats">The threats that are hiding on this website, if any.</param>
        /// <returns></returns>
        public async Task Verdict(string uuid, VerdictScope scope, string scopeValue, VerdictType type, string comment, string[] brands, ThreatType[] threats)
            => await Verdict(new VerdictPayload()
            {
                UUID = uuid,
                Scope = scope,
                ScopeValue = scopeValue,
                Verdict = type,
                Comment = comment,
                Brands = brands,
                ThreatTypes = threats
            });

        /// <summary>
        /// Submit a community verdict to a scan to tell others whether the website is safe or malicious.
        /// </summary>
        /// <param name="payload">The verdict payload object with all the necessary properties.</param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task Verdict(VerdictPayload payload)
        {
            if (!UsesAccountSID) throw new UnauthorizedException();

            if (payload is null) throw new ArgumentNullException(nameof(payload), "Verdict payload is null.");
            if (payload.Brands is null) throw new ArgumentNullException(nameof(payload.UUID), "Brands can't be null, but you can provide an empty array.");
            if (string.IsNullOrEmpty(payload.UUID)) throw new ArgumentNullException(nameof(payload.UUID), "Result UUID is null or empty.");
            if (string.IsNullOrEmpty(payload.ScopeValue)) throw new ArgumentNullException(nameof(payload.ScopeValue), "Scope value is null or empty.");
            if (string.IsNullOrEmpty(payload.Comment)) throw new ArgumentNullException(nameof(payload.Comment), "Comment is null or empty.");
            if (payload.Comment.Length < 40) throw new ArgumentOutOfRangeException(nameof(payload.Comment), "Comment is too short (<40 characters).");
            if (payload.Comment.Length > 500) throw new ArgumentOutOfRangeException(nameof(payload.Comment), "Comment is too long (>500 characters).");

            payload.Brands = payload.Brands.Select(x => x.ToLower()).ToArray();

            await Client.Request($"{URL}/result/verdict/", HttpMethod.Post, JsonSerializer.Serialize(payload, VerdictJsonOptions), sid: _sid);
        }
    }
}