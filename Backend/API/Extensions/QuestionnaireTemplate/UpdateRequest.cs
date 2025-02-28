using API.DTO.Requests.QuestionnaireTemplate;
using Database.Models;

namespace API.Extensions.QuestionnaireTemplate;

public static class UpdateRequest
{
    public static QuestionnaireOptionModel ToModel(this UpdateOption updateRequest, QuestionnaireOptionModel existingEntity)
    {
        return new QuestionnaireOptionModel
        {
            Id = existingEntity.Id,
            OptionValue = updateRequest.OptionValue,
            DisplayText = updateRequest.DisplayText,
        };
    }

    public static QuestionnaireQuestionModel ToModel(this UpdateQuestion updateRequest, QuestionnaireQuestionModel existingEntity)
    {
        return new()
        {
            Id = existingEntity.Id,
            Prompt = updateRequest.Prompt,
            AllowCustom = updateRequest.AllowCustom,
            Options = [.. updateRequest.Options.Select(o => o.ToModel(existingEntity.Options.FirstOrDefault(e => e.Id == o.Id) ?? new QuestionnaireOptionModel{OptionValue = o.OptionValue, DisplayText = o.DisplayText}))]
        };
    }

    public static QuestionnaireTemplateModel ToModel(this UpdateTemplate updateRequest, QuestionnaireTemplateModel existingEntity)
    {
        return new()
        {
            Id = existingEntity.Id,
            Title = updateRequest.Title,
            CreatedAt = existingEntity.CreatedAt,
            LastUpated = DateTime.UtcNow,
            Questions = [.. updateRequest.Questions.Select(q => q.ToModel(existingEntity.Questions.FirstOrDefault(e => e.Id == q.Id) ?? new QuestionnaireQuestionModel{Prompt = q.Prompt, AllowCustom = q.AllowCustom}))],
        };
    }
}
