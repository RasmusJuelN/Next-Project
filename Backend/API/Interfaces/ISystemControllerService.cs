namespace API.Interfaces;

public interface ISystemControllerService
{
    Task<bool> StopServer();
    Task<FileResult> ExportSettings();
    Task<bool> ImportSettings(IFormFile file);
    Task<FileResult> GetLogFile(string filename);
    List<string> GetLogFileNames();
    Task<SettingsFetchResponse> GetSettings();
    Task<SettingsSchema> GetSettingsSchema();
    Task<bool> UpdateSettings(UpdateSettingsRequest rootSettings);
    Task<bool> PatchSettings(PatchSettingsRequest rootSettings);
}
