using API.Models.Requests;
using Database.Models;

namespace API.Extensions;

public static class QuestionnaireTemplateUpdateRequestExtensions
{
    public static QuestionnaireOptionModel ToModel(this QuestionnaireTemplateOptionUpdateRequest option)
    {
        return new QuestionnaireOptionModel
        {
            Id = option.Id,
            OptionValue = option.OptionValue,
            DisplayText = option.DisplayText,
        };
    }

    public static QuestionnaireQuestionModel ToModel(this QuestionnaireTemplateQuestionUpdateRequest question)
    {
        return new QuestionnaireQuestionModel
        {
            Id = question.Id,
            Prompt = question.Prompt,
            AllowCustom = question.AllowCustom,
            Options = [.. question.Options.Select(o => o.ToModel())]
        };
    }

    public static QuestionnaireTemplateModel ToModel(this QuestionnaireTemplateUpdateRequest template)
    {
        return new QuestionnaireTemplateModel
        {
            Id = template.Id,
            TemplateTitle = template.TemplateTitle,
            Questions = [.. template.Questions.Select(q => q.ToModel())]
        };
    }
}
