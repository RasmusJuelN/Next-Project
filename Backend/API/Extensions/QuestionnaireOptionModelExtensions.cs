using API.Models.Responses;
using Database.Models;

namespace API.Extensions;

public static class QuestionnaireOptionModelExtensions
{
    public static QuestionnaireTemplateOptionDto ToDto(this QuestionnaireOptionModel option)
    {
        return new QuestionnaireTemplateOptionDto
        {
            Id = option.Id,
            OptionValue = option.OptionValue,
            DisplayText = option.DisplayText,
        };
    }
}
