using System.ComponentModel.DataAnnotations;
using Database.DTO.QuestionnaireTemplate;
using Database.Enums;
using Database.Extensions;
using Database.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Repository;

/// <summary>
/// Implements repository operations for questionnaire template management.
/// Provides comprehensive functionality for template lifecycle management including creation, modification, retrieval, and deletion with complex hierarchical data handling.
/// </summary>
/// <remarks>
/// This repository manages the complete lifecycle of questionnaire templates, handling complex nested structures
/// of questions and options. It supports both full updates and partial patches while maintaining data integrity
/// and performance through optimized database operations and careful change tracking.
/// </remarks>
/// <param name="context">The database context for data access operations.</param>
/// <param name="loggerFactory">Factory for creating loggers for diagnostic and monitoring purposes.</param>
public class QuestionnaireTemplateRepository(Context context, ILoggerFactory loggerFactory) : IQuestionnaireTemplateRepository
{
    private readonly Context _context = context;
    private readonly GenericRepository<QuestionnaireTemplateModel> _genericRepository = new(context, loggerFactory);

    /// <summary>
    /// Updates an existing questionnaire template with new data, replacing all content.
    /// </summary>
    /// <param name="id">The ID of the questionnaire template to update.</param>
    /// <param name="updatedTemplate">The QuestionnaireTemplateUpdate DTO containing the new template data.</param>
    /// <returns>The updated QuestionnaireTemplate with modifications applied.</returns>
    /// <exception cref="Exception">Thrown when the template with the specified ID is not found.</exception>
    /// <remarks>
    /// This operation completely replaces the existing template content including all questions and options.
    /// It efficiently manages the addition, modification, and removal of nested entities by comparing IDs
    /// and removing orphaned records. The operation maintains referential integrity throughout the update process.
    /// </remarks>
    public async Task<QuestionnaireTemplate> Update(Guid id, QuestionnaireTemplateUpdate updatedTemplate)
    {
        QuestionnaireTemplateModel existingTemplate = await _genericRepository.GetSingleAsync(t => t.Id == id,
            query => query.Include(t => t.Questions).ThenInclude(q => q.Options)) ?? throw new Exception("Template not found.");

        existingTemplate.Title = updatedTemplate.Title;
        existingTemplate.Description = updatedTemplate.Description;

        HashSet<int> updatedQuestionIds = [.. updatedTemplate.Questions.Where(q => q.Id is not null).Select(q => q.Id!.Value)];
        IEnumerable<QuestionnaireQuestionModel> oldQuestions = existingTemplate.Questions.Where(e => !updatedQuestionIds.Contains(e.Id));

        // Remove existing questions
        _context.RemoveRange(oldQuestions);

        foreach (QuestionnaireQuestionUpdate updatedQuestion in updatedTemplate.Questions)
        {
            if (updatedQuestion.Options.Count > 10)
                throw new Exception("A question can have at most 10 options.");
                 
            QuestionnaireQuestionModel? existingQuestion = existingTemplate.Questions.SingleOrDefault(q => q.Id == updatedQuestion.Id);

            if (existingQuestion is not null)
            {
                // Update existing question and check its options
                existingQuestion.Prompt = updatedQuestion.Prompt;
                existingQuestion.AllowCustom = updatedQuestion.AllowCustom;
                existingQuestion.SortOrder = updatedQuestion.SortOrder;

                HashSet<int> updatedOptionIds = [.. updatedQuestion.Options.Where(o => o.Id is not null).Select(o => o.Id!.Value)];
                IEnumerable<QuestionnaireOptionModel> oldOptions = existingQuestion.Options.Where(e => !updatedOptionIds.Contains(e.Id));

                // Remove existing options
                _context.RemoveRange(oldOptions);

                List<QuestionnaireOptionModel> newOptions = [];

                foreach (QuestionnaireOptionUpdate updatedOption in updatedQuestion.Options)
                {
                    QuestionnaireOptionModel? existingOption = existingQuestion.Options.SingleOrDefault(o => o.Id == updatedOption.Id);

                    if (existingOption is not null)
                    {
                        // Update existing option
                        existingOption.OptionValue = updatedOption.OptionValue;
                        existingOption.DisplayText = updatedOption.DisplayText;
                        existingOption.SortOrder = updatedOption.SortOrder;
                    }
                    else
                    {
                        // Create new option
                        QuestionnaireOptionModel newOption = updatedOption.ToModel();
                        existingQuestion.Options.Add(newOption);
                    }
                }
            }
            else
            {
                // Create new question
                QuestionnaireQuestionModel newQuestion = updatedQuestion.ToModel();
                existingTemplate.Questions.Add(newQuestion);
            }
        }

        return existingTemplate.ToDto();
    }

    /// <summary>
    /// Applies partial updates to an existing questionnaire template.
    /// </summary>
    /// <param name="id">The ID of the questionnaire template to patch.</param>
    /// <param name="patchedTemplate">The QuestionnaireTemplatePatch DTO containing the partial updates.</param>
    /// <returns>The updated QuestionnaireTemplate with patches applied.</returns>
    /// <exception cref="Exception">Thrown when the template with the specified ID is not found.</exception>
    /// <remarks>
    /// This method allows selective updates to template properties without replacing the entire structure.
    /// Only provided fields in the patch DTO will be updated, preserving existing content for null fields.
    /// Updates the LastUpdated timestamp automatically to track modification history.
    /// </remarks>
    public async Task<QuestionnaireTemplate> Patch(Guid id, QuestionnaireTemplatePatch patchedTemplate)
    {
        // TODO: Add/Port over existing custom SQL exceptions and use here for better error handling.
        QuestionnaireTemplateModel existingTemplate = await _genericRepository.GetSingleAsync(t => t.Id == id,
            query => query.Include(t => t.Questions).ThenInclude(q => q.Options)) ?? throw new Exception("Template not found.");

        existingTemplate.Title = patchedTemplate.TemplateTitle ?? existingTemplate.Title;
        existingTemplate.Description = patchedTemplate.Description ?? existingTemplate.Description;

        if (patchedTemplate.Questions is not null && patchedTemplate.Questions.Count != 0)
        {
            HashSet<int> patchedQuestionIds = [.. patchedTemplate.Questions.Select(q => q.Id)];
            IEnumerable<QuestionnaireQuestionModel> existingQuestions = existingTemplate.Questions.Where(e => patchedQuestionIds.Contains(e.Id));

            foreach (QuestionnaireQuestionModel existingQuestion in existingQuestions)
            {
                QuestionnaireQuestionPatch patchedQuestion = patchedTemplate.Questions.Single(q => q.Id == existingQuestion.Id);

                existingQuestion.Prompt = patchedQuestion.Prompt ?? existingQuestion.Prompt;
                existingQuestion.AllowCustom = patchedQuestion.AllowCustom ?? existingQuestion.AllowCustom;
                existingQuestion.SortOrder = patchedQuestion.SortOrder ?? existingQuestion.SortOrder;

                if (patchedQuestion.Options is not null && patchedQuestion.Options.Count != 0)
                {
                    HashSet<int> patchedOptionIds = [.. patchedQuestion.Options.Select(o => o.Id)];
                    IEnumerable<QuestionnaireOptionModel> existingOptions = existingQuestion.Options.Where(e => patchedOptionIds.Contains(e.Id));

                    foreach (QuestionnaireOptionModel existingOption in existingOptions)
                    {
                        QuestionnaireOptionPatch patchedOption = patchedQuestion.Options.Single(o => o.Id == existingOption.Id);

                        existingOption.OptionValue = patchedOption.OptionValue ?? existingOption.OptionValue;
                        existingOption.DisplayText = patchedOption.DisplayText ?? existingOption.DisplayText;
                        existingOption.SortOrder = patchedOption.SortOrder ?? existingOption.SortOrder;
                    }
                }
            }
        }

        existingTemplate.LastUpated = DateTime.UtcNow;

        return existingTemplate.ToDto();
    }

    /// <summary>
    /// Retrieves basic information about a questionnaire template without detailed question structure.
    /// </summary>
    /// <param name="id">The ID of the questionnaire template to retrieve.</param>
    /// <returns>A QuestionnaireTemplateBase DTO with essential template information, or null if not found.</returns>
    /// <remarks>
    /// This method provides a lightweight view of the template, suitable for list displays and summary views
    /// where the full question structure is not required. More efficient than the full template retrieval.
    /// </remarks>
    public async Task<QuestionnaireTemplateBase?> GetQuestionnaireTemplateBaseAsync(Guid id)
    {
        QuestionnaireTemplateModel? questionnaire = await _genericRepository.GetSingleAsync(t => t.Id == id);
        return questionnaire?.ToBaseDto();
    }

    /// <summary>
    /// Retrieves complete information about a questionnaire template including all questions and options.
    /// </summary>
    /// <param name="id">The ID of the questionnaire template to retrieve.</param>
    /// <returns>A complete QuestionnaireTemplate DTO with all questions and options, or null if not found.</returns>
    /// <remarks>
    /// This method provides the full template structure, suitable for detailed views, editing interfaces,
    /// and questionnaire activation where the complete question set is required. Includes all nested
    /// questions and their associated options through optimized include operations.
    /// </remarks>
    public async Task<QuestionnaireTemplate?> GetFullQuestionnaireTemplateAsync(Guid id)
    {
        QuestionnaireTemplateModel? questionnaire = await _genericRepository.GetSingleAsync(t => t.Id == id,
            query => query.Include(t => t.Questions).ThenInclude(q => q.Options));
        return questionnaire?.ToDto();
    }

    /// <summary>
    /// Creates a new questionnaire template from the provided template data.
    /// </summary>
    /// <param name="questionnaire">The QuestionnaireTemplateAdd DTO containing template creation data.</param>
    /// <returns>The newly created QuestionnaireTemplate with generated ID and metadata.</returns>
    /// <remarks>
    /// This method validates the template structure and creates all associated questions and options.
    /// The created template will initially be unlocked and available for activation. All nested
    /// entities are created in a single transaction to ensure data consistency.
    /// </remarks>
    public async Task<QuestionnaireTemplate> AddAsync(QuestionnaireTemplateAdd questionnaire)
    {
        QuestionnaireTemplateModel questionnaireTemplate = questionnaire.ToModel();
        await _genericRepository.AddAsync(questionnaireTemplate);
        return questionnaireTemplate.ToDto();
    }

    /// <summary>
    /// Permanently deletes a questionnaire template and all associated active questionnaires from the database.
    /// </summary>
    /// <param name="id">The ID of the questionnaire template to delete.</param>
    /// <exception cref="InvalidOperationException">Thrown when the template with the specified ID is not found.</exception>
    /// <remarks>
    /// This operation is irreversible and will cascade delete all associated questions, options, and active questionnaires.
    /// Active questionnaires are explicitly removed first to maintain referential integrity.
    /// Use with caution as this will affect all users who have active instances of this template.
    /// </remarks>
    public async Task DeleteAsync(Guid id)
    {
        QuestionnaireTemplateModel existingTemplate = await _context.QuestionnaireTemplates.Include(q => q.ActiveQuestionnaires).SingleAsync(q => q.Id == id);

        if (existingTemplate.ActiveQuestionnaires.Count != 0)
        {
            _context.ActiveQuestionnaires.RemoveRange(existingTemplate.ActiveQuestionnaires);
        }

        _genericRepository.Delete(existingTemplate);
    }

    /// <summary>
    /// Performs paginated retrieval of questionnaire templates with filtering and sorting options using keyset pagination.
    /// </summary>
    /// <param name="amount">The number of templates to retrieve per page.</param>
    /// <param name="cursorIdPosition">Optional cursor ID for pagination continuation.</param>
    /// <param name="cursorCreatedAtPosition">Optional cursor creation timestamp for pagination continuation.</param>
    /// <param name="sortOrder">The ordering criteria for the results.</param>
    /// <param name="titleQuery">Optional filter by template title (partial match).</param>
    /// <param name="idQuery">Optional filter by specific template ID (partial match).</param>
    /// <returns>A tuple containing the list of template base DTOs and the total count matching the criteria.</returns>
    /// <remarks>
    /// Uses keyset pagination for consistent performance with large datasets. The cursor parameters work together
    /// to maintain stable pagination even when new records are added. Multiple filter options can be combined
    /// to create targeted queries. Returns lightweight base DTOs for efficient list display and navigation.
    /// </remarks>
    public async Task<(List<QuestionnaireTemplateBase>, int)> PaginationQueryWithKeyset(
        int amount,
        Guid? cursorIdPosition,
        DateTime? cursorCreatedAtPosition,
        TemplateOrderingOptions sortOrder,
        string? titleQuery,
        Guid? idQuery,
        TemplateStatus? templateStatus)
    {
        IQueryable<QuestionnaireTemplateModel> query = _genericRepository.GetAsQueryable();

        query = sortOrder.ApplyQueryOrdering(query);

        if (!string.IsNullOrEmpty(titleQuery))
        {
            query = query.Where(q => q.Title.Contains(titleQuery));
        }

        if (idQuery is not null)
        {
            query = query.Where(q => q.Id.ToString().Contains(idQuery.ToString()!));
        }

        if (templateStatus.HasValue)
        {
            var status = templateStatus.Value;
            query = query.Where(q => q.TemplateStatus == status);
        }

        int totalCount = await query.CountAsync();

        if (cursorIdPosition is not null && cursorCreatedAtPosition is not null)
        {
            if (sortOrder == TemplateOrderingOptions.CreatedAtAsc)
            {
                query = query.Where(q => q.CreatedAt > cursorCreatedAtPosition
                || q.CreatedAt == cursorCreatedAtPosition && q.Id > cursorIdPosition);
            }
            else
            {
                query = query.Where(q => q.CreatedAt < cursorCreatedAtPosition
                || q.CreatedAt == cursorCreatedAtPosition && q.Id < cursorIdPosition);
            }
        }

        List<QuestionnaireTemplateModel> questionnaireTemplates = await query.Take(amount).ToListAsync();
        List<QuestionnaireTemplateBase> questionnaireTemplateBases = [.. questionnaireTemplates.Select(t => t.ToBaseDto())];

        return (questionnaireTemplateBases, totalCount);
    }
    public async Task<QuestionnaireTemplate> FinalizeAsync(Guid id)
    {
        var existing = await _context.QuestionnaireTemplates
            .SingleOrDefaultAsync(t => t.Id == id)
            ?? throw new Exception("Template not found.");

        if (existing.TemplateStatus != TemplateStatus.Finalized)
        {
            existing.TemplateStatus = TemplateStatus.Finalized;
            existing.LastUpated = DateTime.UtcNow;
        }

        return existing.ToDto();
    }

    /// <summary>
    /// Retrieves questionnaire template bases that both a specific student and teacher have answered.
    /// </summary>
    /// <param name="studentId">The unique identifier (GUID) of the student.</param>
    /// <param name="teacherId">The unique identifier (GUID) of the teacher.</param>
    /// <returns>A list of QuestionnaireTemplateBase DTOs representing templates where both participants have completed their responses.</returns>
    /// <exception cref="ArgumentException">Thrown when studentId or teacherId is empty.</exception>
    /// <remarks>
    /// This method finds all questionnaire templates for which both the specified student and teacher have completed
    /// their portions of active questionnaires. Only templates with completed responses from both participants are included.
    /// Returns lightweight template base DTOs for efficient display and further processing.
    /// Useful for result history and tracking shared questionnaire completion between student-teacher pairs.
    /// Results are ordered by the most recent completion date for better user experience.
    /// </remarks>
    public async Task<List<QuestionnaireTemplateBase>> GetTemplateBasesAnsweredByStudentAsync(Guid studentId, Guid teacherId)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty", nameof(studentId));
            
        if (teacherId == Guid.Empty)
            throw new ArgumentException("Teacher ID cannot be empty", nameof(teacherId));

        // Find all templates for which both the student and teacher have completed their active questionnaires
        var templates = await _context.ActiveQuestionnaires
            .Where(aq => aq.Student != null && aq.Student.Guid == studentId && 
                        aq.Teacher != null && aq.Teacher.Guid == teacherId &&
                        aq.StudentCompletedAt.HasValue && aq.TeacherCompletedAt.HasValue)
            .Select(aq => aq.QuestionnaireTemplate!)
            .Distinct()
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return templates.Select(t => t.ToBaseDto()).ToList();
    }
}
