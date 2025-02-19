using System.ComponentModel;
using API.Enums;

namespace API.Models.Requests;

public record class QuestionnaireTemplateApiRequests
{
    public record class PaginationQuery
    {
        /// <summary>
        /// How many items the pagination query should return.
        /// </summary>
        [DefaultValue(5)]
        public required int PageSize { get; set; }
        
        /// <summary>
        /// The order in which the items should be paginated and queried in.
        /// </summary>
        [DefaultValue(QuestionnaireBaseTemplateOrdering.CreatedAtDesc)]
        public QuestionnaireBaseTemplateOrdering Order { get; set; } = QuestionnaireBaseTemplateOrdering.CreatedAtDesc;
        
        /// <summary>
        /// The title to search for in the templates.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The ID of the exact template. Partial search is not supported.
        /// </summary>
        public Guid? Id { get; set; }
        
        /// <summary>
        /// The cursor for where the query should start/resume from
        /// </summary>
        public string? QueryCursor { get; set; }
    }

    public record class AddTemplate
    {
        public required string TemplateTitle { get; set; }
        public required List<AddQuestion> Questions { get; set; }
    }

    public record class AddQuestion
    {
        public required string Prompt { get; set; }
        public required bool AllowCustom { get; set; }
        public required List<AddOption> Options { get; set; }
    }

    public record class AddOption
    {
        public int Id { get; set; }
        public int OptionValue { get; set; }
        public required string DisplayText { get; set; }
    }   
}
