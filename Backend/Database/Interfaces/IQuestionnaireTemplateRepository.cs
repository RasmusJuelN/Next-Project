using Database.DTO.QuestionnaireTemplate;
using Database.Enums;

namespace Database.Interfaces;

/// <summary>
/// Defines the contract for questionnaire template repository operations.
/// Manages the lifecycle of questionnaire templates including creation, modification, retrieval, and deletion.
/// </summary>
public interface IQuestionnaireTemplateRepository
{
    /// <summary>
    /// Creates a new questionnaire template from the provided template data.
    /// </summary>
    /// <param name="questionnaire">The QuestionnaireTemplateAdd DTO containing template creation data.</param>
    /// <returns>The newly created QuestionnaireTemplate with generated ID and metadata.</returns>
    /// <exception cref="ArgumentException">Thrown when the template data is invalid or incomplete.</exception>
    /// <remarks>
    /// This method validates the template structure and creates all associated questions and options.
    /// </remarks>
    Task<QuestionnaireTemplate> AddAsync(QuestionnaireTemplateAdd questionnaire);

    /// <summary>
    /// Updates an existing questionnaire template with new data, replacing all content.
    /// </summary>
    /// <param name="id">The ID of the questionnaire template to update.</param>
    /// <param name="updatedTemplate">The QuestionnaireTemplateUpdate DTO containing the new template data.</param>
    /// <returns>The updated QuestionnaireTemplate with modifications applied.</returns>
    /// <exception cref="ArgumentException">Thrown when the template ID is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the template is locked and cannot be modified.</exception>
    /// <remarks>
    /// This operation completely replaces the existing template content. Use Patch for partial updates.
    /// </remarks>
    Task<QuestionnaireTemplate> Update(Guid id, QuestionnaireTemplateUpdate updatedTemplate);

    /// <summary>
    /// Applies partial updates to an existing questionnaire template.
    /// </summary>
    /// <param name="id">The ID of the questionnaire template to patch.</param>
    /// <param name="patchedTemplate">The QuestionnaireTemplatePatch DTO containing the partial updates.</param>
    /// <returns>The updated QuestionnaireTemplate with patches applied.</returns>
    /// <exception cref="ArgumentException">Thrown when the template ID is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the template is locked and cannot be modified.</exception>
    /// <remarks>
    /// This method allows selective updates to template properties without replacing the entire structure.
    /// Only provided fields in the patch DTO will be updated, preserving existing content for null fields.
    /// </remarks>
    Task<QuestionnaireTemplate> Patch(Guid id, QuestionnaireTemplatePatch patchedTemplate);
    /// <summary>
    /// Permanently deletes a questionnaire template from the database.
    /// </summary>
    /// <param name="id">The ID of the questionnaire template to delete.</param>
    /// <exception cref="ArgumentException">Thrown when the template ID is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the template has associated active questionnaires and cannot be deleted.</exception>
    /// <remarks>
    /// Templates with active questionnaires cannot be deleted to maintain referential integrity.
    /// This operation is irreversible and will cascade delete all associated questions and options.
    /// </remarks>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Retrieves basic information about a questionnaire template without detailed question structure.
    /// </summary>
    /// <param name="id">The ID of the questionnaire template to retrieve.</param>
    /// <returns>A QuestionnaireTemplateBase DTO with essential template information, or null if not found.</returns>
    /// <remarks>
    /// This method provides a lightweight view of the template, suitable for list displays and summary views
    /// where the full question structure is not required.
    /// </remarks>
    Task<QuestionnaireTemplateBase?> GetQuestionnaireTemplateBaseAsync(Guid id);

    /// <summary>
    /// Retrieves complete information about a questionnaire template including all questions and options.
    /// </summary>
    /// <param name="id">The ID of the questionnaire template to retrieve.</param>
    /// <returns>A complete QuestionnaireTemplate DTO with all questions and options, or null if not found.</returns>
    /// <remarks>
    /// This method provides the full template structure, suitable for detailed views, editing interfaces,
    /// and questionnaire activation where the complete question set is required.
    /// </remarks>
    Task<QuestionnaireTemplate?> GetFullQuestionnaireTemplateAsync(Guid id);

    /// <summary>
    /// Performs paginated retrieval of questionnaire templates with filtering and sorting options using keyset pagination.
    /// </summary>
    /// <param name="amount">The number of templates to retrieve per page.</param>
    /// <param name="cursorIdPosition">Optional cursor ID for pagination continuation.</param>
    /// <param name="cursorCreatedAtPosition">Optional cursor creation timestamp for pagination continuation.</param>
    /// <param name="sortOrder">The ordering criteria for the results.</param>
    /// <param name="titleQuery">Optional filter by template title (partial match).</param>
    /// <param name="idQuery">Optional filter by specific template ID.</param>
    /// <returns>A tuple containing the list of template base DTOs and the total count matching the criteria.</returns>
    /// <remarks>
    /// Uses keyset pagination for consistent performance with large datasets. Combine cursor parameters for proper pagination.
    /// Returns lightweight base DTOs for efficient list display and navigation.
    /// </remarks>
    Task<(List<QuestionnaireTemplateBase>, int)> PaginationQueryWithKeyset(
        int amount,
        Guid? cursorIdPosition,
        DateTime? cursorCreatedAtPosition,
        TemplateOrderingOptions sortOrder,
        string? titleQuery,
        Guid? idQuery,
        TemplateStatus? templateStatus);
        Task<QuestionnaireTemplate> FinalizeAsync(Guid id);

}
