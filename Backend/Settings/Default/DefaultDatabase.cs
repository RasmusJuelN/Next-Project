using Settings.Interfaces;

namespace Settings.Default;

public class DefaultDatabase : IDatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
}
