using API.DTO.Requests.QuestionnaireTemplate;
using Database.Models;

namespace API.Extensions;

public static class QuestionnaireTemplateAddRequestExtensions
{
    public static QuestionnaireTemplateModel ToModel(this AddTemplate questionnaireAddRequest)
    {
        return new QuestionnaireTemplateModel
        {
            TemplateTitle = questionnaireAddRequest.TemplateTitle,
            Questions = [.. questionnaireAddRequest.Questions.Select(q => q.ToModel())]
        };
    }
}
