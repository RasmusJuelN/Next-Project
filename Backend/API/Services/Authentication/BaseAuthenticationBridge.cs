using API.Interfaces;

namespace API.Services.Authentication;

public abstract class BaseAuthenticationBridge(IFieldMappingProvider fieldMappingProvider) : IAuthenticationBridge
{
    protected readonly IFieldMappingProvider _fieldMappingProvider = fieldMappingProvider;
    protected readonly Dictionary<string, Func<object, Type, object>> _converter = fieldMappingProvider.GetFieldConverter();

    public abstract void Authenticate(string username, string password);
    public abstract bool IsConnected();
    public abstract void Dispose();

    public abstract TUser? SearchUser<TUser>(string username) where TUser : new();

    public abstract TGroup? SearchGroup<TGroup>(string groupName) where TGroup : new();

    public abstract TEntity? SearchId<TEntity>(string Id) where TEntity : new();

    public abstract (List<TUser>, string, bool) SearchUserPagination<TUser>(string username, string? userRole, int pageSize, string? sessionId) where TUser : new();

    /// <summary>
    /// Maps authentication system data to a strongly-typed model using provider-specific field mappings.
    /// </summary>
    protected virtual TMappedEntity MapToModel<TMappedEntity>(
        Dictionary<string, object> authenticationData) where TMappedEntity : new()
    {
        var result = new TMappedEntity();
        var properties = typeof(TMappedEntity).GetProperties();
        var fieldMappings = _fieldMappingProvider.GetFieldMappings<TMappedEntity>();

        foreach (var property in properties)
        {
            // Use the field mapping to get the actual field name in the data source
            if (!fieldMappings.TryGetValue(property.Name, out var dataFieldName)) continue;

            if (!authenticationData.TryGetValue(dataFieldName, out var authValue)) continue;
            
            Type type = property.PropertyType;

            object? convertedValue = ConvertAuthenticationValue(authValue, dataFieldName, type);

            if (convertedValue != null)
            {
                property.SetValue(result, convertedValue);
            }
        }

        return result;
    }

    protected virtual object ConvertAuthenticationValue(object authValue, string authField, Type type)
    {
        _converter.TryGetValue(authField, out Func<object, Type, object>? converter);

        if (converter != null)
        {
            return converter(authValue, type);
        }

        return authValue;
    }

    public virtual string[] GetEntriesToQuery<MappedEntity>()
    {
        return [.. _fieldMappingProvider.GetFieldMappings<MappedEntity>().Values];
    }
}