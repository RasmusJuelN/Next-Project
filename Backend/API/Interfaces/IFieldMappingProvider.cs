namespace API.Interfaces;

public interface IFieldMappingProvider
{
    /// <summary>
    /// Gets the field mappings for the specified model type.
    /// </summary>
    /// <typeparam name="TModel">The type of the model for which to get field mappings.</typeparam>
    /// <returns>>A dictionary containing the field mappings.</returns>
    Dictionary<string, string> GetFieldMappings<TModel>();

    Dictionary<string, Func<object, Type, object>> GetFieldConverter();
}
