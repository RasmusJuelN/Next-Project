namespace Database.Models;

internal class CustomAnswerModel
{
    internal int Id { get; set; }
    internal required string Response { get; set; }
    internal required int ActiveQuestionnaireResponseId { get; set; }

    // External navigational properties and references
    internal required ActiveQuestionnaireResponseModel ActiveQuestionnaireResponse { get; set; }
}
