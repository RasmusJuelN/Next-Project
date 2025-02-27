using API.DTO.Requests.QuestionnaireTemplate;
using Database.Models;

namespace API.Extensions.QuestionnaireTemplate;

public static class AddRequest
{
    public static QuestionnaireTemplateModel ToModel(this AddTemplate questionnaireAddRequest)
    {
        return new QuestionnaireTemplateModel
        {
            Title = questionnaireAddRequest.Title,
            Questions = [.. questionnaireAddRequest.Questions.Select(q => q.ToModel())]
        };
    }

    public static QuestionnaireQuestionModel ToModel(this AddQuestion question)
    {
        return new QuestionnaireQuestionModel
        {
            Prompt = question.Prompt,
            AllowCustom = question.AllowCustom,
            Options = [.. question.Options.Select(q => q.ToModel())]
        };
    }

    public static QuestionnaireOptionModel ToModel(this AddOption option)
    {
        return new QuestionnaireOptionModel
        {
            OptionValue = option.OptionValue,
            DisplayText = option.DisplayText
        };
    }
}
