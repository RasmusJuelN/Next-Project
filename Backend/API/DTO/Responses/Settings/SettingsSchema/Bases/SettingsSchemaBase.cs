namespace API.DTO.Responses.Settings.SettingsSchema.Bases;

public record class SettingsSchemaBase
{
    public required string Type { get; set; }
    public required bool Required { get; set; }
    public required string Description { get; set; }
}
