using Database.DTO.ActiveQuestionnaire;
using Database.Models;

namespace Database.Extensions;

public static class UserSpecificActiveQuestionnaireMapper
{
    public static UserSpecificActiveQuestionnaireBase ToBaseDTOAsStudent(this ActiveQuestionnaireModel activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            CompletedAt = activeQuestionnaire.StudentCompletedAt
        };
    }

    public static UserSpecificActiveQuestionnaireBase ToBaseDTOAsTeacher(this ActiveQuestionnaireModel activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            CompletedAt = activeQuestionnaire.StudentCompletedAt
        };
    }
}
