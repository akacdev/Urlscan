using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Urlscan
{
    public static class API
    {
        public const int MaxRetries = 3;
        public const int RetryDelay = 1000;
        public const int PreviewMaxLength = 500;

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string url,
            HttpStatusCode target = HttpStatusCode.OK,
            string sid = null,
            bool absoluteUrl = false)
        => await Request(cl, method, url, null, target, sid, absoluteUrl);

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string url,
            object obj,
            HttpStatusCode target = HttpStatusCode.OK,
            JsonSerializerOptions options = null,
            string sid = null)
        => await Request(cl, method, url, new StringContent(JsonSerializer.Serialize(obj, options ?? Constants.JsonOptions), Encoding.UTF8, Constants.JSONContentType), target, sid);

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string url,
            HttpContent content,
            HttpStatusCode target = HttpStatusCode.OK,
            string sid = null,
            bool absoluteUrl = false)
        {
            int retries = 0;

            HttpResponseMessage res = null;

            while (retries < MaxRetries)
            {
                HttpRequestMessage req = new(method, absoluteUrl ? url : $"api/v{UrlscanClient.Version}/{url}")
                {
                    Content = content
                };

                if (sid is not null) req.Headers.Add($"Cookie", $"sid={sid}");

                res = await cl.SendAsync(req);

                retries++;

                if ((int)res.StatusCode < Constants.ErrorStatusCode) break;
                else await Task.Delay(RetryDelay);
            }

            if (!target.HasFlag(res.StatusCode))
            {
                if (res.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    RetryConditionHeaderValue retry = res.Headers.RetryAfter;
                    if (retry is not null) await Task.Delay((int)retry.Delta.Value.TotalMilliseconds);
                }
                else if (res.StatusCode == HttpStatusCode.BadRequest)
                {
                    string json = await res.Content.ReadAsStringAsync();
                    string preview = json[..Math.Min(json.Length, PreviewMaxLength)];

                    try
                    {
                        UrlscanError err = JsonSerializer.Deserialize<UrlscanError>(json);

                        throw err.Message switch
                        {
                            "DNS Error - Could not resolve domain" => new NXDOMAINException(err.Description),
                            "Don't be silly now ..." => new SillyException(err.Description),
                            _ => new UrlscanException($"Request resulted in the error \"{err.Description}\" with message \"{err.Message}\""),
                        };
                    }
                    catch (JsonException)
                    {
                        throw new UrlscanException($"Requested resulted in a 400 Bad Request, and could not be parsed into JSON. Preview:\n{preview}");
                    }
                }
            }

            retries++;
            if (retries == MaxRetries)
            {
                string text = await res.Content.ReadAsStringAsync();
                string preview = text[..Math.Min(text.Length, PreviewMaxLength)];

                throw new UrlscanException(
                    $"Ran out of retry attempts while requesting {method} {url}, last status code: {res.StatusCode}" +
                    $"\nPreview:" +
                    $"\n{preview}");
            }

            return res;
        }

        public static async Task<T> Deseralize<T>(this HttpResponseMessage res, JsonSerializerOptions options = null)
        {
            string json = await res.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json)) throw new UrlscanException("Response content is empty, can't parse as JSON.");

            try
            {
                return JsonSerializer.Deserialize<T>(json, options ?? Constants.JsonOptions);
            }
            catch (Exception ex)
            {
                throw new($"Exception while parsing JSON: {ex.GetType().Name} => {ex.Message}\nJSON preview: {json[..Math.Min(json.Length, PreviewMaxLength)]}");
            }
        }
    }
}