using API.DTO.Requests.QuestionnaireTemplate;
using Database.Models;

namespace API.Extensions;

public static class QuestionnaireTemplateOptionAddRequestExtensions
{
    public static QuestionnaireOptionModel ToModel(this AddOption option)
    {
        return new QuestionnaireOptionModel
        {
            OptionValue = option.OptionValue,
            DisplayText = option.DisplayText
        };
    }
}
