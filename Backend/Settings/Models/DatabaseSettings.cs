
namespace Settings.Models;

public class DatabaseSettings : Base, IDatabaseSettings
{
    [JsonIgnore]
    public override string Key { get; } = "Database";
    
    [Description("Connection string used to connect to the database.")]
    public string ConnectionString { get; set; } = string.Empty;
}
