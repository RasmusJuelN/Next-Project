using Database.DTO.User;

namespace Database.DTO.ActiveQuestionnaire;

public record class ActiveQuestionnaireAdd
{
    public required string Title { get; set; }
    public string? Description { get; set; }
}
