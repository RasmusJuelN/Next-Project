using System.Text.Json.Serialization;

namespace Settings.Models;

public abstract class Base
{
    [JsonIgnore]
    public abstract string Key { get; }
}
