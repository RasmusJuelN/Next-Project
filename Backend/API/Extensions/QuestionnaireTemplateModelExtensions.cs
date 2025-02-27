using API.DTO.Responses.QuestionnaireTemplate;
using Database.Models;

namespace API.Extensions;

public static class QuestionnaireTemplateModelExtensions
{
    public static FetchTemplateBase ToBaseDto(this QuestionnaireTemplateModel questionnaireTemplate)
    {
        return new FetchTemplateBase
        {
            Id = questionnaireTemplate.Id,
            Title = questionnaireTemplate.Title,
            CreatedAt = questionnaireTemplate.CreatedAt,
            LastUpdated = questionnaireTemplate.LastUpated,
            IsLocked = questionnaireTemplate.IsLocked,
        };
    }

    public static FetchTemplate ToDto(this QuestionnaireTemplateModel questionnaireTemplate)
    {
        return new FetchTemplate
        {
            Id = questionnaireTemplate.Id,
            Title = questionnaireTemplate.Title,
            CreatedAt = questionnaireTemplate.CreatedAt,
            LastUpdated = questionnaireTemplate.LastUpated,
            IsLocked = questionnaireTemplate.IsLocked,
            Questions = [.. questionnaireTemplate.Questions.Select(q => q.ToDto())]
        };
    }

    public static ActiveQuestionnaireModel ToActiveQuestionnaire(
        this QuestionnaireTemplateModel questionnaireTemplate,
        StudentModel student,
        TeacherModel teacher)
    {
        return new ActiveQuestionnaireModel
        {
            Title = questionnaireTemplate.Title,
            Student = student,
            Teacher = teacher,
            QuestionnaireTemplate = questionnaireTemplate,
            ActiveQuestionnaireQuestions = [.. questionnaireTemplate.Questions.Select(q => q.ToActiveQuestionnaireQuestion())]
        };
    }
}