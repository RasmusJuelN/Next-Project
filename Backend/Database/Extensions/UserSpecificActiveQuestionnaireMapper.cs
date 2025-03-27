using Database.DTO.ActiveQuestionnaire;
using Database.Models;

namespace Database.Extensions;

public static class UserSpecificActiveQuestionnaireMapper
{
    public static StudentSpecificActiveQuestionnaire ToBaseDTOAsStudent(this ActiveQuestionnaireModel activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt
        };
    }

    public static StudentSpecificActiveQuestionnaire ToBaseDTOAsTeacher(this ActiveQuestionnaireModel activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            StudentCompletedAt = activeQuestionnaire.TeacherCompletedAt
        };
    }
}
