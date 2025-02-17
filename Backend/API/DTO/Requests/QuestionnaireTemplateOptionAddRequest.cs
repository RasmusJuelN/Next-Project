using System;

namespace API.Models.Requests;

public class QuestionnaireTemplateOptionAddRequest
{
    public required int OptionValue { get; set; }
    public required string DisplayText { get; set; }
}
