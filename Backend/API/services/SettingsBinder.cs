using Settings.Models;

namespace API.services;

public class SettingsBinder(IConfiguration configuration)
{
    /// <summary>
    /// Binds a configuration section to an instance of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to bind the configuration section to. Must inherit from <see cref="Base"/> and have a parameterless constructor.</typeparam>
    /// <returns>An instance of type <typeparamref name="T"/> with the configuration section bound to it.</returns>
    /// <summary>
    /// Binds the configuration section to the specified type.
    /// </summary>
    public T Bind<T>() where T : Base, new()
    {
        T configSection = new();
        configuration.GetSection(configSection.Key).Bind(configSection);
        return configSection;
    }
}
