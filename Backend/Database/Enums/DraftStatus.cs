using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TemplateStatus
{
    Draft,
    Finalized,
    Deleted
}