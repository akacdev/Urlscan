using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Urlscan
{
    public class User
    {
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("lastname")]
        public string LastName { get; set; }

        [JsonPropertyName("submissions")]
        public SubmissionStats Stats { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("isPro")]
        public bool IsPro { get; set; }

        [JsonPropertyName("preferences")]
        public Preferences Preferences { get; set; }

        [JsonPropertyName("limits")]
        public Limits Limits
        {
            get
            {
                if (_limits.API.Count == 0)
                {
                    _limits.API[LimitType.Private] = _limits.Private;
                    _limits.API[LimitType.Public] = _limits.Public;
                    _limits.API[LimitType.Retrieve] = _limits.Retrieve;
                    _limits.API[LimitType.Search] = _limits.Search;
                    _limits.API[LimitType.Unlisted] = _limits.Unlisted;
                    _limits.API[LimitType.Livescan] = _limits.Livescan;
                    _limits.API[LimitType.Liveshot] = _limits.Liveshot;
                }

                return _limits;
            }
            set
            {
                _limits = value;
            }
        }

        private Limits _limits;
    }

    public class SubmissionStats
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("private")]
        public int Private { get; set; }

        [JsonPropertyName("unlisted")]
        public int Unlisted { get; set; }

        [JsonPropertyName("public")]
        public int Public { get; set; }

        [JsonPropertyName("api")]
        public int Api { get; set; }

        [JsonPropertyName("manual")]
        public int Manual { get; set; }

        [JsonPropertyName("lastSubmission")]
        public DateTime LastSubmission { get; set; }
    }

    public class Limits
    {
        public Dictionary<LimitType, Limit> API { get; set; } = new();

        [JsonPropertyName("private")]
        public Limit @Private { get; set; }

        [JsonPropertyName("public")]
        public Limit @Public { get; set; }

        [JsonPropertyName("retrieve")]
        public Limit Retrieve { get; set; }

        [JsonPropertyName("search")]
        public Limit Search { get; set; }

        [JsonPropertyName("unlisted")]
        public Limit Unlisted { get; set; }

        [JsonPropertyName("livescan")]
        public Limit Livescan { get; set; }

        [JsonPropertyName("liveshot")]
        public Limit Liveshot { get; set; }

        [JsonPropertyName("maxRetentionPeriodDays")]
        public int MaxRetentionPeriodDays { get; set; }

        [JsonPropertyName("maxSearchRangeMonths")]
        public int MaxSearchRangeMonths { get; set; }

        [JsonPropertyName("maxSearchResults")]
        public int MaxSearchResults { get; set; }
    }

    public class Limit
    {
        [JsonPropertyName("day")]
        public int Day { get; set; }

        [JsonPropertyName("hour")]
        public int Hour { get; set; }

        [JsonPropertyName("minute")]
        public int Minute { get; set; }
    }

    public class Preferences
    {
        [JsonPropertyName("defaultVisibility")]
        public string DefaultVisibility { get; set; }

        [JsonPropertyName("enforceVisibility")]
        public bool EnforceVisibility { get; set; }
    }
}
