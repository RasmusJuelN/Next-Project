using System.Text.Json;

namespace API.services;

public class Serializer
{
    private readonly JsonSerializerOptions s_writeOptions;
    private readonly JsonSerializerOptions s_readOptions;

    public Serializer() 
    {
        s_writeOptions = new()
        {
            WriteIndented = true
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
