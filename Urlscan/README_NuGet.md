# Urlscan

![](https://raw.githubusercontent.com/actually-akac/Urlscan/master/Urlscan/icon.png)

🔎 An async and lightweight C# library for interacting with the Urlscan API.

## Usage
Provides an easy interface for interacting with the Urlscan API.

You can use this library to automate your Urlscan submissions, search for existing scans, track newly submitted scans and analyse network activity of malicious websites.

To get started, add the library into your solution with either the `NuGet Package Manager` or the `dotnet` CLI.
```rust
dotnet add package Urlscan
```

For the primary classes to become available, import the used namespace.
```csharp
using Urlscan;
```

Need more examples? Under the `Example` directory you can find a working demo project that implements this library.

### Obtaining API keys
> API keys can be created in the user section `Settings & API`

> Secure identifier SID cookies can be obtained from your browser's cookie storage. Make sure to only copy the value, without the name! 

## Features
- Built for **.NET 6** and **.NET 7**
- Fully **async**
- Extensive **XML documentation**
- Coverage of the free API endpoints, including some user-only and beta routes  
- **No external dependencies** (uses integrated HTTP and JSON)
- **Custom exceptions** (`UrlscanException`) for advanced catching
- Automatic **ratelimit** handling and request **retries**
- Example project to demonstrate all capabilities of the library
- Scan suspicious URLs and submit verdicts on them
- Download screenshots and page DOMs
- Empower your threat intelligence with live scans through `LiveClient`

## Code Samples

### Initializing a new API client
```csharp
UrlscanClient client = new("d339bf51-9bc7-4d6a-a960-a264896b1891");
```

### Getting the current user
```csharp
User user = await client.GetCurrentUser();
```

### Submitting a new scan
```csharp
Submission subm = await client.Scan(new ScanParameters()
{
    Url = "https://example.com",
    Tags = new string[] { "test" },
    Country = ScanCountry.FI,
    UserAgent = "My-Custom-Scanner/1.0.0",
    OverrideSafety = false,
    Referer = "https://google.com",
    Visibility = Visibility.Public
});
```

### Polling a submission for the result
```csharp
Result res = await client.Poll(subm);
```

### Getting similar scans
```csharp
SimilarScan[] similar = await client.GetSimilarScans("bc1ef5f2-eddc-40ae-86c9-fb5894b5d1f2");
```

### Processing newly created scans in real time
```csharp
LiveClient live = new();
live.UrlScanned += (sender, scan) =>
{
    Console.WriteLine(scan.Task.URL[..Math.Min(scan.Task.URL.Length, 50)]);
};
```

### Adding a verdict to a scan
```csharp
await client.AddVerdict(new VerdictParameters()
{
    UUID = "8964cc71-ea31-476c-ba8f-863bf4bf6b2f",
    Comment = "Running a Discord phishing scam with Discord HypeSquad as their target.",
    Scope = VerdictScope.PageDomain,
    ScopeValue = "contact-hype-testers.com",
    ThreatTypes = new ThreatType[]
    {
        ThreatType.Phishing,
        ThreatType.BrandImpersonation
    },
    Brands = new string[]
    {
        "Discord"
    },
    Verdict = VerdictType.Malicious
});
```

## Available Methods
- Task **AddVerdict**(Result result, VerdictScope scope, VerdictType type, string comment, string[] brands, ThreatType[] threats)
- Task **AddVerdict**(string uuid, VerdictScope scope, string scopeValue, VerdictType type, string comment, string[] brands, ThreatType[] threats)
- Task **AddVerdict**(VerdictParameters parameters)
- Task\<byte[]> **DownloadScreenshot**(Result result)
- Task\<byte[]> **DownloadScreenshot**(string uuid)
- Task\<byte[]> **Liveshot**(string url, int width = 1280, int height = 1024)
- Task\<Result> **GetResult**(string uuid)
- Task\<Result> **Poll**(string uuid, int delay = 5000, int interval = 2000)
- Task\<Result> **Poll**(Submission submission, int delay = 5000, int interval = 2000)
- Task\<SearchResult[]> **Search**(string query, int amount = 100)
- Task\<SimilarScan[]> **GetSimilarScans**(string uuid)
- Task\<Stats> **GetStats**()
- Task\<Stream> **DownloadScreenshotStream**(string uuid)
- Task\<Stream> **LiveshotStream**(string url, int width = 1280, int height = 1024)
- Task\<string> **DownloadDOM**(Result result)
- Task\<string> **DownloadDOM**(string uuid)
- Task\<Submission> **Scan**(string url, string[] tags = null, string userAgent = null, string referer = null, bool overrideSafety = false, Visibility visibility = Visibility.Public, ScanCountry country = ScanCountry.Auto)
- Task\<Submission> **Scan**(ScanParameters parameters)
- Task\<User> **GetCurrentUser**()

## Available Events
- EventHandler\<LiveScan> `UrlScanned`

## References
- https://urlscan.io
- https://twitter.com/urlscanio