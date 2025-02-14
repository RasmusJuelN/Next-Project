using System.ComponentModel;
using API.Enums;
using API.Models.Responses;

namespace API.Models.Requests;

public record class QuestionnaireBaseTemplateRequests
{
    public record class Get
    {
        public required int PageSize { get; set; }
        
        [DefaultValue(QuestionnaireBaseTemplateOrdering.CreatedAtDesc)]
        public QuestionnaireBaseTemplateOrdering Order { get; set; } = QuestionnaireBaseTemplateOrdering.CreatedAtDesc;
        public string? Title { get; set; }
        public Guid? Id { get; set; }
        public QuestionnaireTemplateBaseDto.NextCursor? NextCursor { get; set; }
    }
    public record class Patch
    {
        public string? TemplateTitle { get; set; }
        public List<QuestionnaireTemplateQuestionUpdateRequest>? Questions { get; set; }
    }
}
