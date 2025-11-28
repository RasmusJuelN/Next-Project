
namespace Database.Extensions;

public static class QuestionnaireGroupMapper
{
    public static QuestionnaireGroupBase ToBaseDto(this QuestionnaireGroupModel group)
    {
        return new()
        {
            GroupId = group.GroupId,
            TemplateId = group.TemplateId,
            Name = group.Name,
            CreatedAt = group.CreatedAt,
            TemplateTitle = group.Template?.Title,
            QuestionnaireCount = group.Questionnaires?.Count ?? 0
        };
    }
}
