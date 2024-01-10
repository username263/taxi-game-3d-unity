using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaxiGame3D.Server.Converters;

public class BooleanConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True:
                return true;
            case JsonTokenType.False:
                return false;
            case JsonTokenType.String:
                return reader.GetString()?.ToLower() switch
                {
                    "true" => true,
                    "false" => false,
                    _ => throw new JsonException()
                };
            case JsonTokenType.Number:
                return reader.GetInt16() != 0;
            default:
                throw new JsonException();
        }
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}
