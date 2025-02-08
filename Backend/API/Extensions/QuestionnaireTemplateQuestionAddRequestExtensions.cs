using API.Models.Requests;
using Database.Models;

namespace API.Extensions;

public static class QuestionnaireTemplateQuestionAddRequestExtensions
{
    public static QuestionnaireQuestionModel ToModel(this QuestionnaireTemplateQuestionAddRequest question)
    {
        return new QuestionnaireQuestionModel
        {
            Prompt = question.Prompt,
            AllowCustom = question.AllowCustom,
            Options = [.. question.QuestionnaireTemplateOptions.Select(q => q.ToModel())]
        };
    }
}
