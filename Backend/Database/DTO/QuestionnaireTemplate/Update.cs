using API.Attributes;

namespace Database.DTO.QuestionnaireTemplate;

/// <summary>
/// Represents a data transfer object for updating an existing questionnaire option.
/// Inherits from QuestionnaireOptionAdd and adds an optional identifier for the option to be updated.
/// </summary>
/// <remarks>
/// This record is used when modifying existing questionnaire options. The Id property
/// allows identification of the specific option to update, while inheriting all
/// properties from QuestionnaireOptionAdd for the updated values.
/// </remarks>
public record class QuestionnaireOptionUpdate : QuestionnaireOptionAdd
{
    public int? Id { get; set; } = null;
}

/// <summary>
/// Represents a data transfer object for updating an existing questionnaire question.
/// Inherits from QuestionnaireQuestionAdd and adds update-specific properties.
/// </summary>
/// <remarks>
/// This record is used when modifying an existing questionnaire question, providing
/// the question ID for identification and allowing updates to the question's options.
/// </remarks>
public record class QuestionnaireQuestionUpdate : QuestionnaireQuestionAdd
{
    public int? Id { get; set; } = null;
    
    [MaxQuestionOptions]
    new public List<QuestionnaireOptionUpdate> Options { get; set; } = [];
}

/// <summary>
/// Represents a data transfer object for updating an existing questionnaire template.
/// Inherits from QuestionnaireTemplateAdd and overrides the Questions property to use
/// QuestionnaireQuestionUpdate objects for modification operations.
/// </summary>
public record class QuestionnaireTemplateUpdate : QuestionnaireTemplateAdd
{
    new public List<QuestionnaireQuestionUpdate> Questions { get; set; } = [];
}