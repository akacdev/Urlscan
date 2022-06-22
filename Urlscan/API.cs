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
        public const int MaxRetries = 5;
        public const int RetryDelay = 1000 * 3;
        public const int ExtraDelay = 1000;
        public const int PreviewMaxLength = 500;

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            string url,
            HttpMethod method,
            string json,
            HttpStatusCode target = HttpStatusCode.OK,
            string sid = null)
        => await Request(cl, url, method, new StringContent(json, Encoding.UTF8, "application/json"), target, sid);

        public static async Task<HttpResponseMessage> RequestSID
        (
            this HttpClient cl,
            string sid,
            string url,
            HttpMethod method,
            HttpContent content = null,
            HttpStatusCode target = HttpStatusCode.OK
        )
        => await Request(cl, url, method, content, target, sid);

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            string url,
            HttpMethod method,
            HttpContent content = null,
            HttpStatusCode target = HttpStatusCode.OK,
            string sid = null)
        {
            int retries = 0;

            HttpResponseMessage res = null;

            while (res is null || !target.HasFlag(res.StatusCode))
            {
                HttpRequestMessage req = new(method, url);
                req.Content = content;
                if (sid is not null) req.Headers.Add($"Cookie", $"sid={sid}");

                res = await cl.SendAsync(req);

                if (!target.HasFlag(res.StatusCode))
                {
                    if (res.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        RetryConditionHeaderValue retry = res.Headers.RetryAfter;
                        if (retry is not null) await Task.Delay(((int)retry.Delta.Value.TotalMilliseconds) + ExtraDelay);
                    }
                    else if (res.StatusCode == HttpStatusCode.BadRequest)
                    {
                        string json = await res.Content.ReadAsStringAsync();

                        try
                        {
                            UrlscanError err = JsonSerializer.Deserialize<UrlscanError>(json);

                            throw err.Message switch
                            {
                                "DNS Error - Could not resolve domain" => new NxDomainException(err.Description),
                                "Don't be silly now ..." => new SillyException(err.Description),
                                _ => new($"Request resulted in the error \"{err.Description}\" with message \"{err.Message}\""),
                            };
                        }
                        catch (JsonException)
                        {
                            throw new($"Requested resulted in a 400 Bad Request, and could not be parsed into JSON.\n{json}");
                        }

                        throw new($"Request resulted in a Bad Request, full response: {json}");
                    }
                    else await Task.Delay(RetryDelay);
                }

                retries++;
                if (retries == MaxRetries) throw new($"Ran out of retry attempts while requesting {method} {url}, last status code: {res.StatusCode}");
            }

            return res;
        }

        public static async Task<T> Deseralize<T>(this HttpResponseMessage res, JsonSerializerOptions options = null)
        {
            string json = await res.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json)) throw new("Response content is empty, can't parse as JSON.");

            try
            {
                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch (Exception ex)
            {
                throw new($"Exception while parsing JSON: {ex.GetType().Name} => {ex.Message}\nJSON preview: {json[..Math.Min(json.Length, PreviewMaxLength)]}");
            }
        }
    }
}
