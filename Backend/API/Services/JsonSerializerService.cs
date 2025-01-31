using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Services;

public class JsonSerializerService
{
    private readonly JsonSerializerOptions s_writeOptions;
    private readonly JsonSerializerOptions s_readOptions;

    public JsonSerializerService() 
    {
        s_writeOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        s_readOptions = new()
        {
            AllowTrailingCommas = true
        };
    }

    public string Serialize<T>(T data)
    {
        return JsonSerializer.Serialize(data, s_writeOptions);
    }

    public T? DeSerialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, s_readOptions);
    }
}
