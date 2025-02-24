using API.DTO.Responses.QuestionnaireTemplate;
using Database.Models;

namespace API.Extensions;

public static class QuestionnaireTemplateModelExtensions
{
    public static FetchTemplateBase ToBaseDto(this QuestionnaireTemplateModel questionnaireTemplate)
    {
        return new FetchTemplateBase
        {
            Id = questionnaireTemplate.Id,
            TemplateTitle = questionnaireTemplate.TemplateTitle,
            CreatedAt = questionnaireTemplate.CreatedAt,
            LastUpdated = questionnaireTemplate.LastUpated,
            IsLocked = questionnaireTemplate.IsLocked,
        };
    }

    public static FetchTemplate ToDto(this QuestionnaireTemplateModel questionnaireTemplate)
    {
        return new FetchTemplate
        {
            Id = questionnaireTemplate.Id,
            TemplateTitle = questionnaireTemplate.TemplateTitle,
            CreatedAt = questionnaireTemplate.CreatedAt,
            LastUpdated = questionnaireTemplate.LastUpated,
            IsLocked = questionnaireTemplate.IsLocked,
            Questions = [.. questionnaireTemplate.Questions.Select(q => q.ToDto())]
        };
    }
}