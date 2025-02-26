using API.DTO.Responses.QuestionnaireTemplate;
using Database.Models;

namespace API.Extensions;

public static class QuestionnaireOptionModelExtensions
{
    public static FetchOption ToDto(this QuestionnaireOptionModel option)
    {
        return new FetchOption
        {
            Id = option.Id,
            OptionValue = option.OptionValue,
            DisplayText = option.DisplayText,
        };
    }

    public static ActiveQuestionnaireOptionModel ToActiveQuestionnaireOption(this QuestionnaireOptionModel option)
    {
        return new ActiveQuestionnaireOptionModel
        {
            OptionValue = option.OptionValue,
            DisplayText = option.DisplayText
        };
    }
}
