using Database.DTO.QuestionnaireTemplate;
using Database.Models;

namespace Database.Extensions;

public static class QuestionnaireTemplateAddMapper
{
    public static QuestionnaireTemplateModel ToModel(this QuestionnaireTemplateAdd questionnaire)
    {
        return new()
        {
            Title = questionnaire.Title,
            Description = questionnaire.Description,
            Questions = [.. questionnaire.Questions.Select(q => q.ToModel())]
        };
    }

    public static QuestionnaireQuestionModel ToModel(this QuestionnaireQuestionAdd question)
    {
        return new()
        {
            Prompt = question.Prompt,
            AllowCustom = question.AllowCustom,
            Options = [.. question.Options.Select(o => o.ToModel())]
        };
    }

    public static QuestionnaireOptionModel ToModel(this QuestionnaireOptionAdd option)
    {
        return new()
        {
            OptionValue = option.OptionValue,
            DisplayText = option.DisplayText
        };
    }
}
