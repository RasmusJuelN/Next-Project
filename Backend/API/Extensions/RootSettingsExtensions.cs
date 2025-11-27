using System.Collections;

namespace API.Extensions;

public static class RootSettingsExtensions
{
    public static T MergeWith<T>(this T current, T defaultValues) where T : class
    {
        MergeSettings(current, defaultValues);
        return current;
    }
    
    /// <summary>
    /// Recursively merges default values into current settings where current values are null or empty.
    /// </summary>
    /// <param name="current">The current settings object to be updated</param>
    /// <param name="defaultValues">The default settings object containing fallback values</param>
    private static void MergeSettings(object current, object defaultValues)
    {
        if (current == null || defaultValues == null) return;
        
        var currentType = current.GetType();
        var defaultType = defaultValues.GetType();
        
        // Only merge if types match
        if (currentType != defaultType) return;
        
        var properties = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite);
        
        foreach (var property in properties)
        {
            var currentValue = property.GetValue(current);
            var defaultValue = property.GetValue(defaultValues);
            
            // Skip if default value is null or if it's the Version property (handled separately)
            if (defaultValue == null || property.Name == nameof(RootSettings.Version)) 
                continue;
            
            if (ShouldUseDefaultValue(currentValue, property.PropertyType))
            {
                property.SetValue(current, defaultValue);
            }
            else if (IsComplexObject(property.PropertyType) && currentValue != null)
            {
                // Recursively merge nested objects
                MergeSettings(currentValue, defaultValue);
            }
        }
    }

    /// <summary>
    /// Determines if the current value should be replaced with the default value.
    /// </summary>
    private static bool ShouldUseDefaultValue(object? currentValue, Type propertyType)
    {
        if (currentValue == null) return true;
        
        // Handle strings
        if (propertyType == typeof(string))
        {
            return string.IsNullOrWhiteSpace((string)currentValue);
        }
        
        // Handle collections (List, Dictionary, etc.)
        if (currentValue is ICollection collection)
        {
            return collection.Count == 0;
        }
        
        // Handle value types with default values
        if (propertyType.IsValueType)
        {
            var defaultValueForType = Activator.CreateInstance(propertyType);
            return currentValue.Equals(defaultValueForType);
        }
        
        return false;
    }

    /// <summary>
    /// Determines if a type is a complex object that should be recursively merged.
    /// </summary>
    private static bool IsComplexObject(Type type)
    {
        return !type.IsValueType && 
            type != typeof(string) && 
            !typeof(ICollection).IsAssignableFrom(type) &&
            !type.IsEnum;
    }
}
