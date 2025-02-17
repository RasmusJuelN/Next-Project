using API.Models.Requests;
using Database.Models;

namespace API.Extensions;

public static class QuestionnaireTemplateOptionAddRequestExtensions
{
    public static QuestionnaireOptionModel ToModel(this QuestionnaireTemplateApiRequests.AddOption option)
    {
        return new QuestionnaireOptionModel
        {
            OptionValue = option.OptionValue,
            DisplayText = option.DisplayText
        };
    }
}
