namespace API.DTO.Responses.Settings.SettingsSchema.Bases;

public record class SettingsSchemaExtended : SettingsSchemaBase
{
    public required object DefaultValue { get; set; }
}
