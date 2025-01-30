namespace Database.Models;

public class CustomAnswerModel
{
    public int Id { get; set; }
    public required string Response { get; set; }
    public required int ActiveQuestionnaireResponseId { get; set; }

    // External navigational properties and references
    public required ActiveQuestionnaireResponseModel ActiveQuestionnaireResponse { get; set; }
}
