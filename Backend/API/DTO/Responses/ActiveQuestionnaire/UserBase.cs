namespace API.DTO.Responses.ActiveQuestionnaire;

public record class UserBase
{
    public required string UserName { get; set; }
    public required string FullName { get; set; }
}
