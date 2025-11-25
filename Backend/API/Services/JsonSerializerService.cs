using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Services;

/// <summary>
/// Provides standardized JSON serialization and deserialization services with pre-configured options.
/// This service ensures consistent JSON formatting across the application with proper enum handling
/// and formatting preferences for both reading and writing operations.
/// </summary>
/// <remarks>
/// The service maintains separate configurations for serialization and deserialization:
/// <list type="bullet">
/// <item><description>Write operations use indented formatting and camelCase enum conversion</description></item>
/// <item><description>Read operations allow trailing commas for flexibility</description></item>
/// </list>
/// This separation allows for optimal performance and compatibility in different scenarios.
/// </remarks>
public class JsonSerializerService
{
    private readonly JsonSerializerOptions s_writeOptions;
    private readonly JsonSerializerOptions s_readOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonSerializerService"/> class with pre-configured serialization options.
    /// </summary>
    /// <remarks>
    /// This constructor sets up two distinct sets of options:
    /// <list type="bullet">
    /// <item><description>Write options: Enables indented formatting and camelCase enum conversion for readable output</description></item>
    /// <item><description>Read options: Allows trailing commas to handle flexible input formats</description></item>
    /// </list>
    /// </remarks>
    public JsonSerializerService() 
    {
        s_writeOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        s_readOptions = new()
        {
            AllowTrailingCommas = true,
            //  Add JsonStringEnumConverter to allow converting string values in JSON to enum types.
            // Ensures that camelCase strings like "active" correctly map to TestEnum.Active.
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) } 

        };
    }

    /// <summary>
    /// Serializes an object to a JSON string using the configured write options.
    /// </summary>
    /// <typeparam name="T">The type of object to serialize.</typeparam>
    /// <param name="data">The object to serialize to JSON.</param>
    /// <returns>A JSON string representation of the input object with indented formatting.</returns>
    /// <remarks>
    /// This method uses write-optimized options that include:
    /// <list type="bullet">
    /// <item><description>Indented formatting for human readability</description></item>
    /// <item><description>CamelCase enum value conversion</description></item>
    /// </list>
    /// The output is suitable for API responses and configuration files.
    /// </remarks>
    /// <exception cref="NotSupportedException">Thrown when the type cannot be serialized.</exception>
    /// <exception cref="ArgumentException">Thrown when the object contains circular references.</exception>
    public string Serialize<T>(T data)
    {
        return JsonSerializer.Serialize(data, s_writeOptions);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of the specified type using the configured read options.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON string to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>
    /// An instance of type <typeparamref name="T"/> populated with data from the JSON string,
    /// or <c>null</c> if the JSON represents a null value or cannot be deserialized.
    /// </returns>
    /// <remarks>
    /// This method uses read-optimized options that include:
    /// <list type="bullet">
    /// <item><description>Trailing comma tolerance for flexible input parsing</description></item>
    /// <item><description>Standard property name matching</description></item>
    /// </list>
    /// The method is designed to handle JSON from external sources and configuration files.
    /// </remarks>
    /// <exception cref="JsonException">Thrown when the JSON is invalid or cannot be parsed.</exception>
    /// <exception cref="NotSupportedException">Thrown when the target type cannot be deserialized.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the JSON string is null.</exception>
    public T? DeSerialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, s_readOptions);
    }
}
