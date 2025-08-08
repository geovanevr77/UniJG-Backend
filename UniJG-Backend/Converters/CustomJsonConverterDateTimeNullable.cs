using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using UniJG.Application;
using UniJG_Backend.Constants;

namespace UniJG_Backend.Converters
{
    public class CustomJsonConverterDateTimeNullable : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                DateTime dataInformada = DateTime.ParseExact(
                    reader.GetString()[..Date.MaskLength],
                    Date.MaskFormat,
                    CultureInfo.InvariantCulture);

                return TimeZoneInfo.ConvertTimeToUtc(
                    dateTime: dataInformada,
                    sourceTimeZone: TimeZoneInfo.FindSystemTimeZoneById(TimeZones.Brasilia));
            } catch (Exception)
            {
                throw new JsonException("Data inválida");
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(WriteValue(value.Value));
        }

        internal static DateTimeOffset WriteValue(DateTime value)
        {
            TimeZoneInfo timezoneBrasilia = TimeZoneInfo.FindSystemTimeZoneById(TimeZones.Brasilia);
            TimeSpan timeSpanBrasilia = timezoneBrasilia.GetUtcOffset(value);

            value = value.Kind switch
            {
                DateTimeKind.Unspecified => TimeZoneInfo.ConvertTimeFromUtc(
                    dateTime: value,
                    destinationTimeZone: timezoneBrasilia),

                DateTimeKind.Utc => TimeZoneInfo.ConvertTimeFromUtc(
                    dateTime: value,
                    destinationTimeZone: timezoneBrasilia),

                DateTimeKind.Local => TimeZoneInfo.ConvertTimeFromUtc(
                     dateTime: value.ToUniversalTime(),
                     destinationTimeZone: timezoneBrasilia),

                _ => value
            };

            return new DateTimeOffset(value, timeSpanBrasilia);
        }
    }
}