using Database.DTO.ActiveQuestionnaire;
using Database.Models;

namespace Database.Extensions;

public static class ActiveQuestionnaireMapper
{
    public static ActiveQuestionnaireBase ToBaseDto(this ActiveQuestionnaireModel activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            Student = activeQuestionnaire.Student.ToBaseDto(),
            Teacher = activeQuestionnaire.Teacher.ToBaseDto(),
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt,
            TeacherCompletedAt = activeQuestionnaire.TeacherCompletedAt
        };
    }

    public static ActiveQuestionnaire ToDto(this ActiveQuestionnaireModel activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            ActivatedAt = activeQuestionnaire.ActivatedAt,
            Student = activeQuestionnaire.Student.ToBaseDto(),
            Teacher = activeQuestionnaire.Teacher.ToBaseDto(),
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt,
            TeacherCompletedAt = activeQuestionnaire.TeacherCompletedAt,
            Questions = [.. activeQuestionnaire.QuestionnaireTemplate.Questions.Select(q => q.ToDto())]
        };
    }

    public static FullResponse ToFullResponse(this ActiveQuestionnaireModel activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            Student = new() { User = activeQuestionnaire.Student.ToDto(), CompletedAt = (DateTime)activeQuestionnaire.StudentCompletedAt!},
            Teacher = new() { User = activeQuestionnaire.Teacher.ToDto(), CompletedAt = (DateTime)activeQuestionnaire.TeacherCompletedAt!},
            Answers = [.. activeQuestionnaire.StudentAnswers.Zip(activeQuestionnaire.TeacherAnswers).Select(a => new FullAnswer {
                Question = a.First.Question!.Prompt,
                StudentResponse = a.First.CustomResponse ?? a.First.Option!.DisplayText,
                IsStudentResponseCustom = a.First.CustomResponse is not null,
                TeacherResponse = a.Second.CustomResponse ?? a.Second.Option!.DisplayText,
                IsTeacherResponseCustom = a.Second.CustomResponse is not null
            })]
        };
    }
    public static FullResponseDate ToFullResponseDate(this ActiveQuestionnaireModel activeQuestionnaire)
    {
        return new()
        {
            Id = activeQuestionnaire.Id,
            Title = activeQuestionnaire.Title,
            Description = activeQuestionnaire.Description,
            StudentCompletedAt = activeQuestionnaire.StudentCompletedAt,
            Answers = [.. activeQuestionnaire.StudentAnswers.Zip(activeQuestionnaire.TeacherAnswers).Select(a => new FullAnswer {
                Question = a.First.Question!.Prompt,
                StudentResponse = a.First.CustomResponse ?? a.First.Option!.DisplayText,
                IsStudentResponseCustom = a.First.CustomResponse is not null,
                TeacherResponse = a.Second.CustomResponse ?? a.Second.Option!.DisplayText,
                IsTeacherResponseCustom = a.Second.CustomResponse is not null
            })]
        };
    }
}
