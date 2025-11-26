using API.Attributes;

namespace Database.DTO.QuestionnaireTemplate;

/// <summary>
/// Represents a data transfer object for patching questionnaire option properties.
/// Contains partial data that can be used to update specific fields of an existing questionnaire option.
/// </summary>
/// <remarks>
/// This record is designed for PATCH operations where only specific properties need to be updated.
/// Properties with null values will not be updated in the target entity.
/// </remarks>
public record class QuestionnaireOptionPatch
{
    public int Id { get; set; }
    public int? OptionValue { get; set; }
    public string? DisplayText { get; set; }
    public int? SortOrder { get; set; }
}

/// <summary>
/// Represents a partial update (patch) for a questionnaire question.
/// Contains nullable properties to allow selective updates of question fields.
/// </summary>
/// <remarks>
/// This record is typically used in HTTP PATCH operations where only specific
/// properties need to be updated without affecting other question properties.
/// </remarks>
public record class QuestionnaireQuestionPatch
{
    public int Id { get; set; }
    public string? Prompt { get; set; }
    public bool? AllowCustom { get; set; }
    public int? SortOrder { get; set; }
    
    [MaxQuestionOptions]
    public List<QuestionnaireOptionPatch>? Options { get; set; }
}

/// <summary>
/// Represents a patch object for updating a questionnaire template with partial data.
/// This record is used for PATCH operations where only specific fields need to be updated.
/// </summary>
/// <remarks>
/// All properties are nullable to support partial updates. Only properties with non-null values
/// will be applied during the patch operation.
/// </remarks>
public record class QuestionnaireTemplatePatch
{
    public string? TemplateTitle { get; set; }
    public string? Description { get; set; }
    public List<QuestionnaireQuestionPatch>? Questions { get; set; }
}