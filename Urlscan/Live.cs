using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace Urlscan
{
    /// <summary>
    /// The secondary API client specifically for the live scans endpoint.<br/>
    /// This polls the API on regular intervals and delivers you newly created scans through events.<br/>
    /// Primairly meant to be used by people who wish to process newly created scans for their own phishing threat intelligence.
    /// </summary>
    public class LiveClient
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

        private EventHandler<LiveScan> Handler;
        public event EventHandler<LiveScan> UrlScanned
        {
            add
            {
                Handler += value;
                if (Handler.GetInvocationList().Length == 1) Start();
            }
            remove
            {
                Handler -= value;
                if (Handler is null || Handler.GetInvocationList().Length == 0) Stop();
            }
        }

        private static int Interval;
        private static int Size;

        /// <summary>
        /// Create a new instance of the client for polling live results.
        /// </summary>
        /// <param name="interval">How often newly created scans should be retrieved, in milliseconds.</param>
        /// <param name="size"></param>
        public LiveClient(int interval = 30000, int size = 100)
        {
            if (interval < 3000) throw new ArgumentOutOfRangeException(nameof(interval), "Poll interval has to be at least 3000ms.");
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), "Poll size has to be at least 1.");

            Interval = interval;

            Size = size;
            Seen = new(Size);

            Client.DefaultRequestHeaders.AcceptEncoding.ParseAdd(Constants.AcceptedEncoding);
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(Constants.LiveUserAgent);
            Client.DefaultRequestHeaders.Accept.ParseAdd(Constants.JsonContentType);

            Timer = new()
            {
                Interval = Interval
            };
            Timer.Elapsed += async (o, e) => await Poll();
        }

        private readonly Timer Timer;

        /// <summary>
        /// Forbicly start polling Urlscan's live endpoint. 
        /// </summary>
        public void Start()
        {
            _ = Poll();
            Timer.Start();
        }
        /// <summary>
        /// Forcibly stop polling Urlscan's live endpoint. Polling is automatically stopped when all event listeners are removed.
        /// </summary>
        public void Stop() => Timer.Stop();

        /// <summary>
        /// A cache for already seen scans to prevent duplicates.
        /// </summary>
        private readonly List<string> Seen;

        /// <summary>
        /// A counter for failed requests in a row to stop polling in case Urlscan is having an incident.
        /// </summary>
        private int FailedRequests = 0;

        /// <summary>
        /// Poll for the next chunk of recently finished submissions. 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task Poll()
        {
            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"json/live?size={Size}", absoluteUrl: true);

            if (res.StatusCode == HttpStatusCode.OK) FailedRequests = 0;
            else
            {
                FailedRequests++;

                if (FailedRequests >= 5)
                {
                    Stop();
                    throw new Exception($"Urlscan is likely having some issues right now, disabling Live polling.");
                }
            } 

            LiveContainer cont = await res.Deseralize<LiveContainer>();

            LiveScan[] unseen = cont.Results.Where(x => !Seen.Contains(x.Task.UUID)).ToArray();

            foreach (LiveScan scan in unseen)
            {
                string uuid = scan.Task.UUID;
                Seen.Add(uuid);

                OnScan(scan);
            }

            List<string> toRemove = new();

            foreach (string uuid in Seen)
            {
                if (cont.Results.All(x => x.Task.UUID != uuid)) toRemove.Add(uuid);
            };

            foreach (string uuid in toRemove) Seen.Remove(uuid);
        }

        public void OnScan(LiveScan scan)
        {
            Handler.Invoke(this, scan);
        }
    }
}