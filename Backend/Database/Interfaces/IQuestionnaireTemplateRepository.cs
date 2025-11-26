
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
    /// Soft deletes a questionnaire template by marking it as deleted instead of removing it from the database.
    /// </summary>
    /// <param name="id">The ID of the questionnaire template to soft delete.</param>
    /// <exception cref="ArgumentException">Thrown when the template ID is not found.</exception>
    /// <remarks>
    /// This operation marks the template as deleted by setting its status to TemplateStatus.Deleted
    /// and updating the LastUpdated timestamp. The template remains in the database for audit purposes
    /// but is excluded from normal queries. This approach maintains referential integrity and allows
    /// for potential recovery of deleted templates if needed.
    /// </remarks>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Permanently deletes a questionnaire template and all associated active questionnaires from the database.
    /// </summary>
    /// <param name="id">The ID of the questionnaire template to permanently delete.</param>
    /// <exception cref="ArgumentException">Thrown when the template ID is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the template cannot be deleted.</exception>
    /// <remarks>
    /// This operation is irreversible and will cascade delete all associated questions, options, and active questionnaires.
    /// Active questionnaires are explicitly removed first to maintain referential integrity.
    /// Use with caution as this will affect all users who have active instances of this template.
    /// Consider using DeleteAsync for soft deletion in most scenarios.
    /// </remarks>
    Task HardDeleteAsync(Guid id);

    /// <summary>
    /// Undeletes a soft deleted questionnaire template by restoring its status.
    /// </summary>
    /// <param name="id">The ID of the questionnaire template to undelete.</param>
    /// <returns>The restored QuestionnaireTemplate with updated status.</returns>
    /// <exception cref="ArgumentException">Thrown when the template ID is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the template is not in deleted state.</exception>
    /// <remarks>
    /// This operation restores a previously soft deleted template. The template status will be set to Draft
    /// unless there are active questionnaires associated with it, in which case it will be set to Finalized.
    /// Updates the LastUpdated timestamp to track the restoration.
    /// </remarks>
    Task<QuestionnaireTemplate> UndeleteAsync(Guid id);

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

    /// <summary>
    /// Retrieves questionnaire template bases that both a specific student and teacher have answered.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="teacherId">The unique identifier (GUID) of the teacher.</param>
    /// <returns>A list of QuestionnaireTemplateBase DTOs representing templates where both participants have completed their responses.</returns>
    /// <remarks>
    /// This method finds all questionnaire templates for which both the specified student and teacher have completed
    /// their portions of active questionnaires. Only templates with completed responses from both participants are included.
    /// Returns lightweight template base DTOs for efficient display and further processing.
    /// Useful for result history and tracking shared questionnaire completion between student-teacher pairs.
    /// </remarks>
    Task<List<QuestionnaireTemplateBase>> GetTemplateBasesAnsweredByStudentAsync(Guid studentId, Guid teacherId);

}
