using API.Models.Requests;
using Database.Models;

namespace API.Extensions;

public static class QuestionnaireTemplateAddRequestExtensions
{
    public static QuestionnaireTemplateModel ToModel(this QuestionnaireTemplateApiRequests.AddTemplate questionnaireAddRequest)
    {
        return new QuestionnaireTemplateModel
        {
            TemplateTitle = questionnaireAddRequest.TemplateTitle,
            Questions = [.. questionnaireAddRequest.Questions.Select(q => q.ToModel())]
        };
    }
}
