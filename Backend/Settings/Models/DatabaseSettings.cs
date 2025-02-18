using Settings.Interfaces;

namespace Settings.Models;

public class DatabaseSettings : Base, IDatabaseSettings
{
    public override string Key { get; } = "Database";
    
    public string ConnectionString { get; set; } = string.Empty;
}
