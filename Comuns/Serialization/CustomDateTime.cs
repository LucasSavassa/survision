using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using System.Diagnostics;
using System.Buffers.Text;

namespace Comuns.Serialization
{
    internal class CustomDateTime : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTime));

            if (DateTime.TryParseExact(reader.GetString() ?? string.Empty, "yyyy/MM/ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime value))
            {
                return value;
            }
            else
            {
                throw new FormatException("Unable to deserialize token to datetime because format is not 'yyyy/MM/ddTHH:mm:ss'.");
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy/MM/ddTHH:mm:ss", CultureInfo.InvariantCulture));
        }
    }
}
