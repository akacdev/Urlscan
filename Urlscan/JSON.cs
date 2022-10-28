using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Urlscan
{
    public class LongUnixConverter : JsonConverter<DateTime>
    {
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.ToUnixSeconds());
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => reader.GetInt64().ToDate(),
                _ => throw new JsonException(),
            };
        }

    }

    public class StringIntConverter : JsonConverter<int>
    {
        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(Convert.ToInt32(value));
        }

        public override int Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => int.Parse(reader.GetString()),
                _ => throw new JsonException(),
            };
        }

    }

    public class KebabCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToKebabCase();
    }

    public class LowerCaseNamingPolicy : JsonNamingPolicy
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