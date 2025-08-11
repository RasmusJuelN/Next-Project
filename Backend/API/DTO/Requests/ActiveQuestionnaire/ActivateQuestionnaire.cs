namespace API.DTO.Requests.ActiveQuestionnaire;

public record class ActivateQuestionnaire
{
    public List<Guid> StudentIds { get; set; }
    public List<Guid> TeacherIds { get; set; }
    public required Guid TemplateId { get; set; }
}
