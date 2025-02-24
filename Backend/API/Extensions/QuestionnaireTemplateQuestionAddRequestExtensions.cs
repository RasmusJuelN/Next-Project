using API.DTO.Requests.QuestionnaireTemplate;
using Database.Models;

namespace API.Extensions;

public static class QuestionnaireTemplateQuestionAddRequestExtensions
{
    public static QuestionnaireQuestionModel ToModel(this AddQuestion question)
    {
        return new QuestionnaireQuestionModel
        {
            Prompt = question.Prompt,
            AllowCustom = question.AllowCustom,
            Options = [.. question.Options.Select(q => q.ToModel())]
        };
    }
}
