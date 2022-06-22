using System;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Urlscan
{
    public class EnumNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToLower();
    }

    public class VerdictNamingPolicy : JsonNamingPolicy
    {
        public string[] VerdictTypes = Enum.GetNames(typeof(VerdictType));
        public string[] VerdictScopes = Enum.GetNames(typeof(VerdictScope));
        public string[] ThreatTypes = Enum.GetNames(typeof(ThreatType));

        public override string ConvertName(string name)
        {
            if (VerdictTypes.Contains(name)) return name.ToLower();

            if (VerdictScopes.Contains(name)) return name switch
            {
                "PageUrl" => "page.url",
                "PageDomain" => "page.domain",
                "TaskUrl" => "task.url",
                "TaskDomain" => "task.url",
                _ => throw new NotImplementedException($"{name} is an unknown verdict scope")
            };

            if (ThreatTypes.Contains(name))
            {
                bool first = true;
                StringBuilder sb = new();

                foreach (char x in name)
                {
                    char c = char.ToLower(x);

                    if (first)
                    {
                        first = false;
                        sb.Append(c);
                        continue;
                    }

                    bool upper = char.IsUpper(x);

                    if (!upper) sb.Append(c);
                    else sb.Append($" {c}");
                }

                return sb.ToString();
            }

            throw new NotImplementedException($"Unknown enum {name}");
        }
    }
}
