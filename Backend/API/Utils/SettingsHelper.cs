using API.Services;
using Settings.Default;

namespace API.Utils;

public class SettingsHelper(string settingsFile)
{
    private readonly string _settingsFile = settingsFile;
    private DefaultSettings _defaultSettings = new();
    private Serializer _serializer = new();

    public bool SettingsExists()
    {
        return File.Exists(_settingsFile);
    }
    
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
