using API.DTO.Responses.ActiveQuestionnaire;
using Database.Models;

namespace API.Extensions.ActiveQuestionnaire;

public static class FetchRequest
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

    public static FetchActiveQuestionnaire ToDto(this ActiveQuestionnaireModel activeQuestionnaire)
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
            TeacherCompletedAt = activeQuestionnaire.TeacherCompletedAt,
            Questions = [.. activeQuestionnaire.ActiveQuestionnaireQuestions.Select(q => q.ToDto())]
        };
    }

    public static FetchActiveQuestionnaireQuestion ToDto(this ActiveQuestionnaireQuestionModel question)
    {
        return new()
        {
            Id = question.Id,
            Prompt = question.Prompt,
            AllowCustom = question.AllowCustom,
            Options = [.. question.ActiveQuestionnaireOptions.Select(o => o.ToDto())]
        };
    }

    public static FetchActiveQuestionnaireOption ToDto(this ActiveQuestionnaireOptionModel option)
    {
        return new()
        {
            Id = option.Id,
            OptionValue = option.OptionValue,
            DisplayText = option.DisplayText
        };
    }
}
