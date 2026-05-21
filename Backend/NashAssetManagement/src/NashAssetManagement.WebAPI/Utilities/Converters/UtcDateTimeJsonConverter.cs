using System.Text.Json;
using System.Text.Json.Serialization;

namespace NashAssetManagement.WebAPI.Utilities.Converters
{
    public class UtcDateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.SpecifyKind(reader.GetDateTime(), DateTimeKind.Utc);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var utc = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            // ISO 8601 with Z
            writer.WriteStringValue(utc.ToString("O"));
        }
    }
}
