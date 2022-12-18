using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Urlscan
{
    /// <summary>
    /// The primary class for interacting with the Urlscan API. 
    /// </summary>
    public class UrlscanClient
    {
        private static readonly HttpClientHandler HttpHandler = new()
        {
            AutomaticDecompression = DecompressionMethods.All
        };

        private readonly HttpClient Client = new(HttpHandler)
        {
            BaseAddress = Constants.BaseUri,
            DefaultRequestVersion = Constants.HttpVersion
        };

        public readonly bool UsesAccountSID;
        public readonly string Sid;

        /// <summary>
        /// Create a new instance of the client for interacting with the main API.
        /// </summary>
        /// <param name="key">Your Urlscan API key. See the GitHub for instructions on obtaing one.</param>
        /// <param name="sid">
        ///     (<b>Optional</b>) Your account's secure identifier cookie for performing operations not implemented in the official API.
        ///     <para>See the GitHub repository for instructions on obtaining one.</para>
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        public UrlscanClient(string key, string sid = null)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key), "API key is null or empty.");

            UsesAccountSID = sid is not null;
            Sid = sid;

            Client.DefaultRequestHeaders.AcceptEncoding.ParseAdd(Constants.AcceptedEncoding);
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(Constants.UserAgent);
            Client.DefaultRequestHeaders.Accept.ParseAdd(Constants.JsonContentType);
            Client.DefaultRequestHeaders.Add("API-Key", key);
        }

        /// <summary>
        /// Fetch the Urlscan statistics, such as how many scans are currently running.
        /// </summary>
        /// <exception cref="UrlscanException"></exception>
        public async Task<Stats> GetStats()
        {
            HttpResponseMessage res = await Client.Request(HttpMethod.Get, "stats", absoluteUrl: true);

            return await res.Deseralize<Stats>();
        }

        /// <summary>
        /// Submit a new scan of a URL.
        /// </summary>
        /// <param name="url">The URL to scan.</param>
        /// <param name="tags">The custom tags you want to attach to your scan.</param>
        /// <param name="userAgent">The custom User Agent to use in the scan.</param>
        /// <param name="referer">The custom Referer to use in the scan.</param>
        /// <param name="overrideSafety">When true, checking for PII (<b>P</b>ersonally <b>I</b>dentifiable <b>I</b>nformation) will be suppressed. This is dangerous.</param>
        /// <param name="visibility">The visibility of your scan. Defaults to <see cref="Visibility.Public"/></param>
        /// <param name="country">The country to use within your scan.</param>
        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Submission> Scan(
            string url,
            string[] tags = null,
            string userAgent = null,
            string referer = null,
            bool overrideSafety = false,
            Visibility visibility = Visibility.Public,
            ScanCountry country = ScanCountry.Auto
        )
        {
            return await Scan(new ScanParameters()
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
        /// Submit a new scan of a URL.
        /// </summary>
        /// <param name="parameters">The scan payload object with all the necessary parameters.</param>
        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Submission> Scan(ScanParameters parameters)
        {
            if (parameters is null) throw new ArgumentNullException(nameof(parameters), "Scan parameters are null or empty.");
            if (string.IsNullOrEmpty(parameters.Url)) throw new ArgumentNullException(nameof(parameters.Url), "Scan URL is null or empty.");

            if (parameters.Country == ScanCountry.Auto) parameters.Country = null;
            if (parameters.OverrideSafety != true) parameters.OverrideSafety = null;

            HttpResponseMessage res = await Client.Request(HttpMethod.Post, "scan", parameters);

            Submission subm = await res.Deseralize<Submission>();
            if (subm.Message != Constants.SuccessSubmissionMessage) throw new UrlscanException($"Received a success status code when submitting a scan, but an unexpected message: {subm.Message}");

            return subm;
        }

        /// <summary>
        /// Fetch a result.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns>The result, or <see langword="null"></see> if none is found.</returns>
        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Result> GetResult(string uuid)
        {
            if (string.IsNullOrEmpty(uuid)) throw new ArgumentNullException(nameof(uuid), "Submission's UUID is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"result/{uuid}", target: HttpStatusCode.OK | HttpStatusCode.NotFound);
            if (res.StatusCode == HttpStatusCode.NotFound) return null;

            return await res.Deseralize<Result>();
        }

        /// <summary>
        /// Polls for a submission's result until the scan is finished.
        /// </summary>
        /// <param name="submission">The submission object to send.</param>
        /// <param name="delay">The delay until polling starts.</param>
        /// <param name="interval">The interval between individual polls.</param>

        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<Result> Poll(Submission submission, int delay = 5000, int interval = 2000)
        {
            if (submission is null) throw new ArgumentNullException(nameof(submission), "Submission is null.");

            return await Poll(submission.UUID, delay, interval);
        }

        /// <summary>
        /// Polls for a submission's result until the scan is finished.
        /// </summary>
        /// <param name="uuid">The UUID of the scan you want to poll for.</param>
        /// <param name="delay">The delay until polling starts.</param>
        /// <param name="interval">The interval between individual polls.</param>
        /// <exception cref="UrlscanException"></exception>
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

            while (retries < Constants.MaxPollingRetries)
            {
                res = await GetResult(uuid);
                if (res is not null) return res;
                
                await Task.Delay(interval);
                retries++;
            }

            throw new($"Ran out of retry attempts while polling the result of submission {uuid}");
        }

        /// <summary>
        /// Returns basic information about your account.
        /// </summary>

        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="UnauthorizedException"></exception>
        public async Task<User> GetCurrentUser()
        {
            HttpResponseMessage res = await Client.Request(HttpMethod.Get, "user/username", sid: Sid, absoluteUrl: true);

            return await res.Deseralize<User>();
        }

        /// <summary>
        /// Download the screenshot of a scan as a byte array.
        /// </summary>
        /// <param name="result">The result you want to download a screenshot of.</param>
        /// <returns>The PNG screenshot as a byte array, or <see langword="null"></see> if none is found.</returns>
        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<byte[]> DownloadScreenshot(Result result)
        {
            if (result is null) throw new ArgumentNullException(nameof(result), "Result is null.");

            return await DownloadScreenshot(result.Task.UUID);
        }

        /// <summary>
        /// Download the screenshot of a scan as a byte array.
        /// </summary>
        /// <param name="uuid">The UUID of a result you want to download a screenshot of.</param>
        /// <returns>The PNG screenshot as a byte array, or <see langword="null"></see> if none is found.</returns>
        /// <exception cref="UrlscanException"></exception>
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
        /// <param name="uuid">The UUID of a result you want to download a screenshot of.</param>
        /// <returns>The stream of a PNG screenshot, or <see langword="null"></see> if none is found.</returns>
        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Stream> DownloadScreenshotStream(string uuid)
        {
            if (string.IsNullOrEmpty(uuid)) throw new ArgumentNullException(nameof(uuid), "Result's UUID is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"screenshots/{uuid}.png", target: HttpStatusCode.OK | HttpStatusCode.NotFound);
            if (res.StatusCode == HttpStatusCode.NotFound) return null;

            return res.Content.ReadAsStream();
        }

        /// <summary>
        /// Download the liveshot of a URL as a byte array.
        /// </summary>
        /// <param name="url">The URL you want to liveshot.</param>
        /// <param name="width">The width of the screenshot.</param>
        /// <param name="height">The height of the screenshot</param>
        /// <returns>The PNG liveshot, or <see langword="null"></see> if none is found.</returns>
        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<byte[]> Liveshot(string url, int width = 1280, int height = 1024)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url), "URL to liveshot is null or empty.");
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), "Width must be a positive value.");
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), "Height must be a positive value.");

            Stream str = await LiveshotStream(url, width, height);
            if (str is null) return null;

            return ((MemoryStream)str).ToArray();
        }

        /// <summary>
        /// Download the liveshot of a URL as a śtream.
        /// </summary>
        /// <param name="url">The URL you want to liveshot.</param>
        /// <param name="width">The width of the screenshot.</param>
        /// <param name="height">The height of the screenshot</param>
        /// <returns>The stream of a PNG liveshot, or <see langword="null"></see> if none is found.</returns>
        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<Stream> LiveshotStream(string url, int width = 1280, int height = 1024)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url), "URL to liveshot is null or empty.");
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), "Width must be a positive value.");
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), "Height must be a positive value.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"liveshot/?&width={width}&height={height}&url={url}", target: HttpStatusCode.OK | HttpStatusCode.NotFound);
            if (res.StatusCode == HttpStatusCode.NotFound) return null;

            return res.Content.ReadAsStream();
        }

        /// <summary>
        /// Downlod the DOM/content of a scan as a string.
        /// </summary>
        /// <param name="result">The result you want to download the DOM of.</param>
        /// <returns>The DOM, or <see langword="null"></see> if none is found.</returns>
        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<string> DownloadDOM(Result result)
        {
            if (result is null) throw new ArgumentNullException(nameof(result), "Result is null.");

            return await DownloadDOM(result.Task.UUID);
        }

        /// <summary>
        /// Downlod the DOM/content of a scan as a string.
        /// </summary>
        /// <param name="uuid">The UUID of a result you want to download the DOM of.</param>
        /// <returns>The DOM, or <see langword="null"></see> if none is found.</returns>
        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<string> DownloadDOM(string uuid)
        {
            if (string.IsNullOrEmpty(uuid)) throw new ArgumentNullException(nameof(uuid), "Result's UUID is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"dom/{uuid}", target: HttpStatusCode.OK | HttpStatusCode.NotFound);
            if (res.StatusCode == HttpStatusCode.NotFound) return null;
            
            return Encoding.UTF8.GetString(await res.Content.ReadAsByteArrayAsync());
        }

        /// <summary>
        /// Search for past scans using an Elastic Search Query.
        /// <para>
        ///     Docs: <seealso href="https://urlscan.io/docs/search/"/>
        /// </para>
        /// </summary>
        /// <param name="query">The query to search with.</param>
        /// <param name="amount">The amount of search results you want to retrieve. The limit is <c>10000</c> for free accounts.</param>
        /// <param name="targetScan">The UUID of a target scan. If set, search result collection stops early if the <c>UUID</c> is hit.</param>
        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="UnauthorizedException"></exception>
        public async Task<SearchItem[]> Search(string query, int amount = 100, string targetScan = null)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query), "Search query is null or empty.");
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be a positive value.");
            if (query.Contains('*') && !UsesAccountSID) throw new UnauthorizedException();

            string searchAfter = null;
            int chunkSize = amount >= 1000 ? 100 : amount;

            List<SearchItem> results = new(amount);

            while (results.Count < amount)
            {
                if ((amount - results.Count) < chunkSize) chunkSize = amount - results.Count;

                HttpResponseMessage res = await Client.Request(HttpMethod.Get,
                    $"search/?q={query}&size={chunkSize}{(searchAfter is not null ? $"&search_after={searchAfter}" : "")}");

                SearchContainer cont = await res.Deseralize<SearchContainer>();
                if (cont.Results.Length == 0) break;

                JsonElement[] sort = cont.Results.Last().Sort;

                long sort1 = sort[0].GetInt64();
                string sort2 = sort[1].GetString();

                searchAfter = $"{sort1},{sort2}";

                results.AddRange(cont.Results);
                if (targetScan is not null && cont.Results.Any(result => result.Task.UUID == targetScan)) break;
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
        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="UnauthorizedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task AddVerdict(Result result, VerdictScope scope, VerdictType type, string comment, string[] brands, ThreatType[] threats)
        {
            if (result is null) throw new ArgumentNullException(nameof(result), "Result is null");

            string scopeValue = scope switch
            { 
                VerdictScope.PageDomain => result.Page.Domain,
                VerdictScope.PageUrl => result.Page.Url,
                _ => throw new NotImplementedException($"{scope} is an unknown verdict scope.")
            };

            await AddVerdict(new VerdictParameters()
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
        /// <param name="uuid">The ID of the scan to add a verdict.</param>
        /// <param name="scope">The scope that should be verdicted.<para>For example, use <see cref="VerdictScope.TaskDomain"/> when the entire domain is malicious, and <see cref="VerdictScope.TaskUrl"/> if just the specific URL.</para></param>
        /// <param name="scopeValue">The value of the scope you're verdicting. This is either a full URL or a hostname.<para><b>Consider using the method overload that accepts a <c>Result</c> to have this property generated automatically.</b></para></param>
        /// <param name="type">The type of your verdict.</param>
        /// <param name="comment">A short comment about the scan. Has to be at least <c>40</c> characters long.</param>
        /// <param name="brands">The impersonated brands, like <c>Discord</c> or <c>Steam</c>, if any.</param>
        /// <param name="threats">The threats that are hiding on this website, if any.</param>
        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="UnauthorizedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task AddVerdict(string uuid, VerdictScope scope, string scopeValue, VerdictType type, string comment, string[] brands, ThreatType[] threats)
            => await AddVerdict(new VerdictParameters()
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
        /// <param name="parameters">A <see cref="VerdictParameters"/> object with all the necessary properties to submit the verdict.</param>
        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="UnauthorizedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task AddVerdict(VerdictParameters parameters)
        {
            if (!UsesAccountSID) throw new UnauthorizedException();

            if (parameters is null) throw new ArgumentNullException(nameof(parameters), "Verdict payload is null.");
            if (parameters.Brands is null) throw new ArgumentNullException(nameof(parameters.UUID), "Brands can't be null, but you can provide an empty array.");
            if (string.IsNullOrEmpty(parameters.UUID)) throw new ArgumentNullException(nameof(parameters.UUID), "Result UUID is null or empty.");
            if (string.IsNullOrEmpty(parameters.ScopeValue)) throw new ArgumentNullException(nameof(parameters.ScopeValue), "Scope value is null or empty.");
            if (string.IsNullOrEmpty(parameters.Comment)) throw new ArgumentNullException(nameof(parameters.Comment), "Comment is null or empty.");
            if (parameters.Comment.Length < 40) throw new ArgumentOutOfRangeException(nameof(parameters.Comment), "Comment is too short (<40 characters).");
            if (parameters.Comment.Length > 500) throw new ArgumentOutOfRangeException(nameof(parameters.Comment), "Comment is too long (>500 characters).");

            parameters.Brands = parameters.Brands.Select(x => x.ToLower()).ToArray();

            await Client.Request(HttpMethod.Post, "result/verdict/", parameters, options: Constants.VerdictJsonOptions, sid: Sid, absoluteUrl: true);
        }

        /// <summary>
        /// Get scans structurally similar to an existing scan.
        /// </summary>
        /// <param name="uuid">The <c>UUID</c> of the scan to be searched for.</param>
        /// <exception cref="UrlscanException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<SimilarScan[]> GetSimilarScans(string uuid)
        {
            if (string.IsNullOrEmpty(uuid)) throw new ArgumentNullException(nameof(uuid), "UUID of the result to find similar scans for is null or empty.");

            if (Client.DefaultRequestHeaders.Accept.All(x => x.MediaType != Constants.HtmlContentType))
                Client.DefaultRequestHeaders.Accept.ParseAdd(Constants.HtmlContentType);

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"result/{uuid}/related/", sid: Sid, absoluteUrl: true);
            string html = await res.Content.ReadAsStringAsync();

            if (html.Contains(Constants.ZeroSimilarHitsMessage)) return Array.Empty<SimilarScan>();

            string tableFrom = "<table ";
            string tableTo = "</table>";

            string prefix = "Failed to parse similar scans HTML, ";

            int tableStart = html.IndexOf(tableFrom);
            if (tableStart == -1) throw new UrlscanException(string.Concat(prefix, "cannot find the start of the table"));
            tableStart += tableFrom.Length;

            int tableEnd = html.IndexOf(tableTo, tableStart);
            if (tableEnd == -1) throw new UrlscanException(string.Concat(prefix, "cannot find the end of the table"));

            string tableHtml = html[tableStart..tableEnd];

            string[] targetLines = tableHtml.Split('\n').Where(line => line.Contains("a href=")).ToArray();
            SimilarScan[] scans = new SimilarScan[targetLines.Length];

            string hrefFrom = "a href=\"";
            string hrefTo = "\"";

            string titleFrom = "title=\"";
            string titleTo = "\"";

            for (int i = 0; i < targetLines.Length; i++)
            {
                SimilarScan scan = new();
                string targetLine = targetLines[i];

                int hrefStart = targetLine.IndexOf(hrefFrom);
                if (hrefStart == -1) throw new UrlscanException(string.Concat(prefix, $"cannot find the start of the 'a href' element value at index {i}"));
                hrefStart += hrefFrom.Length;

                int hrefEnd = targetLine.IndexOf(hrefTo, hrefStart);
                if (hrefEnd == -1) throw new UrlscanException(string.Concat(prefix, $"cannot find the end of the 'a href' element value at index {i}"));

                string aHref = targetLine[hrefStart..hrefEnd];

                scan.UUID = aHref.Split('/').Where(x => x.Length != 0).LastOrDefault();
                if (scan.UUID is null) throw new UrlscanException(string.Concat(prefix, $"cannot find the simlar scan UUID at index {i}"));

                int titleStart = targetLine.IndexOf(titleFrom);
                if (titleStart == -1) throw new UrlscanException(string.Concat(prefix, $"cannot find the start of the 'title' attribute at index {i}"));
                titleStart += titleFrom.Length;

                int titleEnd = targetLine.IndexOf(titleTo, titleStart);
                if (titleEnd == -1) throw new UrlscanException(string.Concat(prefix, $"cannot find the end of the 'title' attribute at index {i}"));

                scan.Url = targetLine[titleStart..titleEnd];

                scans[i] = scan;
            }

            return scans;
        }
    }
}