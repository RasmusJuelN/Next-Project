using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Utils;

public class JsonSerializerUtility
{
    public static JsonSerializerOptions ConfigureJsonSerializerSettings()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
