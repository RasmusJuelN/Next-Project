using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Settings.Interfaces;

namespace Settings.Models;

public class DBLoggerSettings : Base, IDBLoggerSettings
{
    public override string Key { get; } = "DBLogging";

    [Description("Indicates whether database logging is enabled.")]
    public bool IsEnabled { get; set; }

    [Description("Specifies the log levels for database logging.")]
    public Dictionary<string, LogLevel> LogLevel { get; set; } = [];
}
