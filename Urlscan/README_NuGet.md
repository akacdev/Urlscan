# Urlscan

![](https://raw.githubusercontent.com/actually-akac/Urlscan/master/Urlscan/icon.png)

🔎 A C# library for interacting with the Urlscan API.

## Usage
This library can be downloaded as the package `Urlscan`. The main classes are `UrlscanClient` and `LiveClient`.  

https://www.nuget.org/packages/Urlscan

### Obtaining API keys
> API keys can be created in the user section `Settings & API`

> Security identifier SID cookies can be obtained from the cookie storage. Make sure to only copy the value, without the name! 

## Features
- Fully async
- Full coverage of the free API endpoints, including user-only routes
- Scan suspicious URLs and verdict on them
- Download screenshots and page DOMs
- See finsished public scans in real time using `LiveClient`
- Automatic ratelimit handling
- Detailed documentation

## Available methods
- Task<Stats> `Stats`()
- Task<User> `UserInfo`()
- Task<Submission> `Scan`(string url, string[] tags = null, string userAgent = null, string referer = null, bool overrideSafety = false, Visibility visibility = Visibility.Public, ScanCountry country = ScanCountry.Auto)
- Task<Submission> `Scan`(ScanPayload payload)
- Task<Result> `Poll`(Submission subm, int delay = 5000, int interval = 2000)
- Task<Result> `Poll`(string uuid, int delay = 5000, int interval = 2000)
- Task<Result> `Result`(string uuid)
- Task `Verdict`(Result result, VerdictScope scope, VerdictType type, string comment, string[] brands, ThreatType[] threats)
- Task `Verdict`(VerdictPayload payload)
- Task `Verdict`(string uuid, VerdictScope scope, string scopeValue, VerdictType type, string comment, string[] brands, ThreatType[] threats)
- Task<SearchResult[]> `Search`(string query, int amount = 100)
- Task<Stream> `DownloadScreenshotStream`(string uuid)
- Task<byte[]> `DownloadScreenshot`(Result res)
- Task<byte[]> `DownloadScreenshot`(string uuid)
- Task<string> `DownloadDOM`(Result res)
- Task<string> `DownloadDOM`(string uuid)

## Available events
- EventHandler\<LiveScan> `UrlScanned`

## Official Links
https://urlscan.io
https://twitter.com/urlscanio

## Example
Under the `Example` folder you can find a demo application that works with the library.