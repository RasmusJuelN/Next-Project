using API.Models.Responses;
using Database.Models;

namespace API.Extensions;

public static class QuestionnaireTemplateModelExtensions
{
    public static QuestionnaireTemplateResponse ToDto(this QuestionnaireTemplateModel questionnaireTemplate)
    {
        return new QuestionnaireTemplateResponse
        {
            Id = questionnaireTemplate.Id,
            TemplateTitle = questionnaireTemplate.TemplateTitle,
            CreatedAt = questionnaireTemplate.CreatedAt,
            LastUpdated = questionnaireTemplate.LastUpated,
            IsLocked = questionnaireTemplate.IsLocked,
        };
    }
}
