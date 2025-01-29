using Settings.Models;

namespace API.Services;

public class SettingsBinder(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;
    
    /// <summary>
    /// Binds a configuration section to an instance of the specified type.
    /// <para></para>
    /// If the configuration section has a key, the method will bind the values from the section with the key to the instance.<br />
    /// If the configuration section does not have a key, the method will bind the values from the root of the configuration to the instance.
    /// </summary>
    /// <typeparam name="T">The type of the configuration section to bind. Must inherit from <see cref="Base"/> and have a parameterless constructor.</typeparam>
    /// <returns>An instance of the specified type with the configuration values bound to it.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configuration section does not have a key.</exception>
    public T Bind<T>() where T : Base, new()
    {
        T configSection = new();
        
        if (configSection.Key is null)
        {
            throw new InvalidOperationException("The configuration section must have a key.");
        }

        if (configSection.Key == string.Empty)
        {
            _configuration.Bind(configSection);
        }
        else
        {
            _configuration.GetSection(configSection.Key).Bind(configSection);
        }
        
        return configSection;
    }
}
