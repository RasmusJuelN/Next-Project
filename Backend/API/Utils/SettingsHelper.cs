using API.Services;
using Settings.Default;

namespace API.Utils;

/// <summary>
/// Helper class for managing application settings files, including creation of default settings
/// and checking for settings file existence.
/// </summary>
/// <param name="settingsFile">The file path to the settings file to be managed</param>
public class SettingsHelper(string settingsFile)
{
    private readonly string _settingsFile = settingsFile;
    private DefaultSettings _defaultSettings = new();
    private JsonSerializerService _serializer = new();

    /// <summary>
    /// Determines whether the settings file exists on the file system.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the settings file exists; otherwise, <c>false</c>.
    /// </returns>
    public bool SettingsExists()
    {
        return File.Exists(_settingsFile);
    }

    /// <summary>
    /// Creates a default settings file by serializing the default settings to JSON and writing it to the configured file path.
    /// After creating the file, prompts the user to configure required settings before continuing.
    /// </summary>
    /// <remarks>
    /// This method will overwrite any existing settings file. It handles both normal console input and redirected input
    /// (such as when running in Visual Studio Code debug console) by using appropriate input methods.
    /// The method blocks execution until the user provides input, allowing them time to manually edit the generated settings file.
    /// </remarks>
    public void CreateDefault()
    {
        string json = _serializer.Serialize(_defaultSettings);
        File.WriteAllText(_settingsFile, json);

        // TODO: Maybe check if the required settings are actually set before allowing the user to continue?
        Console.WriteLine(@"
        A new default settings file has been generated. 
        Some settings are required to be set for the application to work.
        Press Enter to continue when completed.
        ");

        // Visual Studio Code can, depending on its configuration, redirect everything to the debug console. This catches that.
        if (Console.IsInputRedirected) Console.Read();
        else Console.ReadKey(true);
    }
}
