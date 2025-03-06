using Database.DTO.QuestionnaireTemplate;
using Database.Models;

namespace Database.Extensions;

public static class QuestionnaireTemplateModelMapper
{
    public static QuestionnaireTemplateBase ToBaseDto(this QuestionnaireTemplateModel questionnaire)
    {
        return new()
        {
            Id = questionnaire.Id,
            Title = questionnaire.Title,
            Description = questionnaire.Description,
            CreatedAt = questionnaire.CreatedAt,
            LastUpdated = questionnaire.LastUpated,
            IsLocked = questionnaire.IsLocked
        };
    }

    public static QuestionnaireTemplate ToDto(this QuestionnaireTemplateModel questionnaire)
    {
        return new()
        {
            Id = questionnaire.Id,
            Title = questionnaire.Title,
            Description = questionnaire.Description,
            CreatedAt = questionnaire.CreatedAt,
            LastUpdated = questionnaire.LastUpated,
            IsLocked = questionnaire.IsLocked,
            Questions = [.. questionnaire.Questions.Select(q => q.ToDto())]
        };
    }

    public static QuestionnaireTemplateQuestion ToDto(this QuestionnaireQuestionModel question)
    {
        return new()
        {
            Id = question.Id,
            Prompt = question.Prompt,
            AllowCustom = question.AllowCustom,
            Options = [.. question.Options.Select(o => o.ToDto())]
        };
    }

    public static QuestionnaireTemplateOption ToDto(this QuestionnaireOptionModel option)
    {
        return new()
        {
            Id = option.Id,
            OptionValue = option.OptionValue,
            DisplayText = option.DisplayText
        };
    }
}
