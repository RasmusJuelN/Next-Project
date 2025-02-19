using API.Models.Requests;
using Database.Models;

namespace API.Extensions;

public static class QuestionnaireTemplateUpdateRequestExtensions
{
    public static QuestionnaireOptionModel ToModel(this QuestionnaireTemplateOptionUpdateRequest updateRequest, QuestionnaireOptionModel existingEntity)
    {
        return new QuestionnaireOptionModel
        {
            Id = existingEntity.Id,
            OptionValue = updateRequest.OptionValue,
            DisplayText = updateRequest.DisplayText,
        };
    }

    public static QuestionnaireQuestionModel ToModel(this QuestionnaireTemplateQuestionUpdateRequest updateRequest, QuestionnaireQuestionModel existingEntity)
    {
        return new()
        {
            Id = existingEntity.Id,
            Prompt = updateRequest.Prompt,
            AllowCustom = updateRequest.AllowCustom,
            Options = [.. updateRequest.Options.Select(o => o.ToModel(existingEntity.Options.FirstOrDefault(e => e.Id == o.Id) ?? new QuestionnaireOptionModel{OptionValue = o.OptionValue, DisplayText = o.DisplayText}))]
        };
    }

    public static QuestionnaireTemplateModel ToModel(this QuestionnaireTemplateUpdateRequest updateRequest, QuestionnaireTemplateModel existingEntity)
    {
        return new()
        {
            Id = existingEntity.Id,
            TemplateTitle = updateRequest.TemplateTitle,
            CreatedAt = existingEntity.CreatedAt,
            LastUpated = DateTime.UtcNow,
            Questions = [.. updateRequest.Questions.Select(q => q.ToModel(existingEntity.Questions.FirstOrDefault(e => e.Id == q.Id) ?? new QuestionnaireQuestionModel{Prompt = q.Prompt, AllowCustom = q.AllowCustom}))],
        };
    }
}
