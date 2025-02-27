using API.DTO.Responses.ActiveQuestionnaire;
using Database.Models;

namespace API.Extensions.ActiveQuestionnaire;

public static class ActiveQuestionnaire
{
    public static FetchActiveQuestionnaireBase ToBaseDto(this ActiveQuestionnaireModel activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            Student = new()
            {
                FullName = activeQuestionnaire.Student.FullName,
                UserName = activeQuestionnaire.Student.UserName
            },
            Teacher = new()
            {
                FullName = activeQuestionnaire.Teacher.FullName,
                UserName = activeQuestionnaire.Teacher.UserName
            },
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt,
            TeacherCompletedAt = activeQuestionnaire.TeacherCompletedAt
        };
    }
}
