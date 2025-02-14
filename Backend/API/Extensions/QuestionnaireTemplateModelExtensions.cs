using API.Models.Responses;
using Database.Models;

namespace API.Extensions;

public static class QuestionnaireTemplateModelExtensions
{
    public static QuestionnaireTemplateBaseDto.TemplateBase ToBaseDto(this QuestionnaireTemplateModel questionnaireTemplate)
    {
        return new QuestionnaireTemplateBaseDto.TemplateBase
        {
            Id = questionnaireTemplate.Id,
            TemplateTitle = questionnaireTemplate.TemplateTitle,
            CreatedAt = questionnaireTemplate.CreatedAt,
            LastUpdated = questionnaireTemplate.LastUpated,
            IsLocked = questionnaireTemplate.IsLocked,
        };
    }

    public static QuestionnaireTemplateDto ToDto(this QuestionnaireTemplateModel questionnaireTemplate)
    {
        return new QuestionnaireTemplateDto
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