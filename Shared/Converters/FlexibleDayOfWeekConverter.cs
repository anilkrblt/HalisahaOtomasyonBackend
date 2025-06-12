using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Converters
{
    public class FlexibleDayOfWeekConverter : JsonConverter<DayOfWeek>
    {
        public override DayOfWeek Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (Enum.TryParse<DayOfWeek>(stringValue, true, out var enumValue))
                {
                    return enumValue;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                var intValue = reader.GetInt32();
                if (Enum.IsDefined(typeof(DayOfWeek), intValue))
                {
                    return (DayOfWeek)intValue;
                }
            }

            throw new JsonException($"Unable to convert value to DayOfWeek: {reader.GetString()}");
        }

        public override void Write(Utf8JsonWriter writer, DayOfWeek value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}