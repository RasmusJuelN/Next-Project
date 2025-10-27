using System.ComponentModel;
using Settings.Interfaces;

namespace Settings.Models;

public class DatabaseSettings : Base, IDatabaseSettings
{
    public override string Key { get; } = "Database";
    
    [Description("Connection string used to connect to the database.")]
    public string ConnectionString { get; set; } = string.Empty;
}
