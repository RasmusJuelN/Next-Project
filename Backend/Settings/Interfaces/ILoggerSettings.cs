using Microsoft.Extensions.Logging;
using Settings.Models;

namespace Settings.Interfaces;

public interface ILoggerSettings<TConsole, TFile, TDB>
{
    public Dictionary<string, LogLevel> LogLevel { get; set; }
    public TConsole Console { get; set; }
    public TFile FileLogger { get; set; }
    public TDB DBLogger { get; set; }
}
