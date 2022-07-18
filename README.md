# Urlscan

<div align="center">
  <img width="256" height="256" src="https://raw.githubusercontent.com/actually-akac/Urlscan/master/Urlscan/icon.png">
</div>

<div align="center">
  🔎 A C# library for interacting with the Urlscan API.
</div>

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
- Task<Stats> `GetStats()`
- Task<User> `GetUserInfo()`
- Task<Submission> `Scan(string url, string[] tags = null, string userAgent = null, string referer = null, bool overrideSafety = false, Visibility visibility = Visibility.Public, ScanCountry country = ScanCountry.Auto)`
- Task<Submission> `Scan(ScanPayload payload)`
- Task<Result> `Poll(Submission subm, int delay = 5000, int interval = 2000)`
- Task<Result> `Poll(string uuid, int delay = 5000, int interval = 2000)`
- Task<Result> `GetResult(string uuid)`
- Task<SearchResult[]> `Search(string query, int amount = 100)`
- Task<byte[]> `DownloadScreenshot(Result res)`
- Task<byte[]> `DownloadScreenshot(string uuid)`
- Task<Stream> `DownloadScreenshotStream(string uuid)`
- Task<string> `DownloadDOM(Result res)`
- Task<string> `DownloadDOM(string uuid)`
- Task<byte[]> `Liveshot(string url, int width = 1280, int height = 1024)`
- Task<Stream> `LiveshotStream(string url, int width = 1280, int height = 1024)`
- Task `AddVerdict(Result result, VerdictScope scope, VerdictType type, string comment, string[] brands, ThreatType[] threats)`
- Task `AddVerdict(VerdictPayload payload)`
- Task `AddVerdict(string uuid, VerdictScope scope, string scopeValue, VerdictType type, string comment, string[] brands, ThreatType[] threats)`

## Available events
- EventHandler\<LiveScan> `UrlScanned`

## Official Links
https://urlscan.io</br>
https://twitter.com/urlscanio

## Example
Under the `Example` folder you can find a demo application that works with the library.
```
Current user info:
Registered: 262 days ago
Last Submissions: 0 days ago
Username: @akac - actually akac
Email: foo@bar.cz
Your defualt scan visibility is public
You are not a PRO subscription member

Limits:
[Private] 5/minute, 50/hour, 50/day
[Public] 60/minute, 500/hour, 5000/day
[Retrieve] 120/minute, 5000/hour, 10000/day
[Search] 120/minute, 1000/hour, 1000/day
[Unlisted] 60/minute, 100/hour, 1000/day
[Livescan] 0/minute, 0/hour, 0/day
[Liveshot] 10/minute, 100/hour, 1000/day

You've made 11674 sumissions, of which 117 were private

Currently running scan tasks: 88
24h stats: public: 126950, unlisted: 65202, private: 486239
Total: 192240

Enter a URL to scan: https://pickedhypesquad.gq/
Submitting a scan
Submission created: 34a0a877-6063-4363-9790-042f96e609e1

Polling for scan result, this will take about 10 seconds.

Page was successfully scanned by a submitter from CZ.

Final URL: https://pickedhypesquad.gq/ on domain pickedhypesquad.gq
This page has the IP address 2606:4700:3030::6815:6bb hosted at IP 2606:4700:3030::6815:6bb (CLOUDFLARENET, US) from US
The server software was identified as cloudflare

31 hashes were generated from the resources
34 links are present on the website
2 IPs are contacted
2 ASNs are contacted
2 countries are contacted

Domains contacted: pickedhypesquad.gq fonts.googleapis.com
Technologies used: React Google Tag Manager OneTrust

Anti-phishing verdict: Page is phishing
Urlscan phishing score: 100
Impersonated brands: Discord
Malicious: True

Community phishing score: 20, categories: phishing
Impersonated brands: Discord
Community Votes: 2
Malicious: True

Done analysing URL

Downloading screenshot (screenshot.png) and DOM (dom.html) to the current directory.

Press any key to submit a verdict to a known Discord phishing site.
Successfully verdicted, see it at: https://urlscan.io/result/8964cc71-ea31-476c-ba8f-863bf4bf6b2f/#verdicts

Press any key to search for scans that contain hypesquad in them.
https://hypesquad-badges.gq/
https://requesthypesquad.com/
http://hypesquad-events-invite.com/
https://hypesquad-ssignupevents.com/
http://requesthypesquad.com/

Press any key to start watching for newly scanned URLs.
https://www.ruthelliscenter.org
https://go.techtarget.com/r/207722864/44038934
http://enews.classicfirearms.com/q/LC1p-Jwm7U0XcWI
https://share.daa.com/index.php/s/nQW3aAJmGENyHMa/
https://www.securevdronline.com
```