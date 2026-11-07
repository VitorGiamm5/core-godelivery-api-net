using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoreGoDelivery.Api.Conveters;

public sealed class CustomDateTimeConverter : JsonConverter<DateTime?>
{
    public override void Write(Utf8JsonWriter writer, DateTime? date, JsonSerializerOptions options)
    {
        if (date is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(date.Value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
    }

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected an ISO 8601 date/time string.");
        }

        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var parsed))
        {
            throw new JsonException("Expected a valid ISO 8601 date/time string.");
        }

        return parsed.UtcDateTime;
    }
}
