using API.DTO.Responses.ActiveQuestionnaire;
using Database.DTO.ActiveQuestionnaire;

namespace API.Extensions;

public static class ActiveQuestionnaireExtensions
{
    public static ActiveQuestionnaireStudentBase ToActiveQuestionnaireStudentDTO(this ActiveQuestionnaireBase activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            Student = activeQuestionnaire.Student,
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt
        };
    }

    public static ActiveQuestionnaireTeacherBase ToActiveQuestionnaireTeacherDTO(this ActiveQuestionnaireBase activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            Student = activeQuestionnaire.Student,
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt,
            Teacher = activeQuestionnaire.Teacher,
            TeacherCompletedAt = activeQuestionnaire.TeacherCompletedAt
        };
    }

    public static ActiveQuestionnaireAdminBase ToActiveQuestionnaireAdminDTO(this ActiveQuestionnaireBase activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            Student = activeQuestionnaire.Student,
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt,
            Teacher = activeQuestionnaire.Teacher,
            TeacherCompletedAt = activeQuestionnaire.TeacherCompletedAt
        };
    }
}
