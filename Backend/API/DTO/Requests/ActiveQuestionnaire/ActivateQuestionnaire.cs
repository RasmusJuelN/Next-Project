namespace API.DTO.Requests.ActiveQuestionnaire;

public record class ActivateQuestionnaire
{
    public required Guid StudentId { get; set; }
    public required Guid TeacherId { get; set; }
    public required Guid TemplateId { get; set; }
}
