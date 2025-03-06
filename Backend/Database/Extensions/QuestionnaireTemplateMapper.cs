using Database.DTO.QuestionnaireTemplate;
using Database.Models;

namespace Database.Extensions;

public static class QuestionnaireTemplateMapper
{
    public static QuestionnaireTemplateModel ToModel(this QuestionnaireTemplate questionnaire)
    {
        return new()
        {
            Title = questionnaire.Title,
            Description = questionnaire.Description,
            CreatedAt = questionnaire.CreatedAt,
            LastUpated = questionnaire.LastUpdated,
            Questions = [.. questionnaire.Questions.Select(q => q.ToModel())]
        };
    }

    public static QuestionnaireQuestionModel ToModel(this QuestionnaireTemplateQuestion question)
    {
        return new()
        {
            Prompt = question.Prompt,
            AllowCustom = question.AllowCustom,
            Options = [.. question.Options.Select(o => o.ToModel())]
        };
    }

    public static QuestionnaireOptionModel ToModel(this QuestionnaireTemplateOption option)
    {
        return new()
        {
            OptionValue = option.OptionValue,
            DisplayText = option.DisplayText
        };
    }

    public static ActiveQuestionnaireModel ToActiveQuestionnaire(this QuestionnaireTemplate questionnaire, QuestionnaireTemplateModel questionnaireModel, StudentModel student, TeacherModel teacher)
    {
        return new()
        {
            Title = questionnaire.Title,
            Description = questionnaire.Description,
            Student = student,
            Teacher = teacher,
            QuestionnaireTemplate = questionnaireModel
        };
    }
}
