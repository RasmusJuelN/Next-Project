using Database.DTO.User;

namespace Database.DTO.ActiveQuestionnaire;

public record class ActiveQuestionnaireBase
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required DateTime ActivatedAt { get; set; }
    public required UserBase Student { get; set; }
    public required UserBase Teacher { get; set; }
    public required DateTime? StudentCompletedAt { get; set; }
    public required DateTime? TeacherCompletedAt { get; set; }
}
