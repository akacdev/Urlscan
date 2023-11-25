using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Urlscan
{
    internal static class API
    {
        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string path,
            HttpStatusCode target = HttpStatusCode.OK,
            string sid = null,
            bool absolutePath = false)
        => await Request(cl, method, path, null, target, sid, absolutePath);

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string path,
            object obj,
            HttpStatusCode target = HttpStatusCode.OK,
            JsonSerializerOptions options = null,
            string sid = null,
            bool absolutePath = false)
        => await Request(cl, method, path, await obj.Serialize(options ?? Constants.JsonOptions), target, sid, absolutePath);

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string path,
            HttpContent content,
            HttpStatusCode target = HttpStatusCode.OK,
            string sid = null,
            bool absolutePath = false)
        {
            using HttpRequestMessage req = new(method, absolutePath ? path : string.Concat($"api/v", Constants.Version, "/", path))
            {
                Content = content
            };

            if (sid is not null) req.Headers.Add($"Cookie", $"sid={sid}");

            HttpResponseMessage res = await cl.SendAsync(req);
            content?.Dispose();

            if (target.HasFlag(res.StatusCode)) return res;

            UrlscanError err = (await res.Deseralize<UrlscanError>())
                ?? throw new UrlscanException($"Failed to request {method} {path}, received status code {res.StatusCode}\nPreview: {await res.GetPreview()}");

            UrlscanException ex = new($"Failed to request {method} {path}, received an API error \"{err.Description}\" with message \"{err.Message}\"")
            {
                Error = err
            };

            if (res.StatusCode == HttpStatusCode.TooManyRequests)
            {
                RetryConditionHeaderValue retry = res.Headers.RetryAfter;
                if (retry is not null) ex.RetryAfter = (int)retry.Delta.Value.TotalMilliseconds;
            }
            
            throw ex;
        }
    }
}