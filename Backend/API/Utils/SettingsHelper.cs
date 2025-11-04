using System.Text.Json;
using API.Extensions;
using Settings.Models;

namespace API.Utils;

/// <summary>
/// Helper class for managing application settings files, including creation of default settings
/// and checking for settings file existence.
/// </summary>
/// <param name="settingsFile">The file path to the settings file to be managed</param>
public class SettingsHelper(string settingsFile)
{
    private readonly string _settingsFile = settingsFile;
    private readonly RootSettings _defaultSettings = new();
    private readonly JsonSerializerOptions _jsonSerializerOptions = JsonSerializerUtility.ConfigureJsonSerializerSettings();

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
        string json = JsonSerializer.Serialize(_defaultSettings, _jsonSerializerOptions);
        Write(_settingsFile, json);

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

    public void CheckSettingsVersion()
    {
        RootSettings currentSettings = JsonSerializer.Deserialize<RootSettings>(Read(_settingsFile), _jsonSerializerOptions) ?? throw new InvalidOperationException("Failed to deserialize settings file.");
        RootSettings defaultSettings = new();
        if (currentSettings.Version == defaultSettings.Version)
        {
            return;
        }
        else if (currentSettings.Version > defaultSettings.Version)
        {
            Console.WriteLine($@"
            The current settings file version ({currentSettings.Version}) is newer than the application supports ({defaultSettings.Version}).
            Unkown settings and values will be ignored.
            Press Enter to continue.
            ");

            if (Console.IsInputRedirected) Console.Read();
            else Console.ReadKey(true);
        }
        else
        {
            Console.WriteLine($@"
            The current settings file version ({currentSettings.Version}) is older than the application supports ({defaultSettings.Version}).
            An attempt will be made to automatically upgrade the settings file.
            Press Enter to continue.
            ");

            if (Console.IsInputRedirected) Console.Read();
            else Console.ReadKey(true);

            Upgrade();
        }
    }

    public void Upgrade()
    {
        RootSettings currentSettings = JsonSerializer.Deserialize<RootSettings>(Read(_settingsFile), _jsonSerializerOptions) ?? throw new InvalidOperationException("Failed to deserialize settings file.");
        RootSettings defaultSettings = new();

        Upgrade(currentSettings, defaultSettings);
    }
    
    public void Upgrade(RootSettings currentSettings, RootSettings defaultSettings)
    {
        currentSettings.MergeWith(defaultSettings);
        currentSettings.Version = defaultSettings.Version;

        string json = JsonSerializer.Serialize(currentSettings, _jsonSerializerOptions);
        Write(_settingsFile, json);
    }

    private static void Write(string path, string data)
    {
        File.WriteAllText(path, data);
    }

    private static string Read(string path)
    {
        return File.ReadAllText(path);
    }
}
