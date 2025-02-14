using API.Models.Responses;
using Database.Models;

namespace API.Extensions;

public static class QuestionnaireQuestionModelExtensions
{
    public static QuestionnaireTemplateQuestionDto ToDto(this QuestionnaireQuestionModel question)
    {
        return new QuestionnaireTemplateQuestionDto
        {
            Id = question.Id,
            Prompt = question.Prompt,
            AllowCustom = question.AllowCustom,
            Options = [.. question.Options.Select(o => o.ToDto())]
        };
    }
}
