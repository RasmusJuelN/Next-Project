using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models;

[Table("CustomAnswer")]
public class CustomAnswerModelBase
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public required string Response { get; set; }
    
    // External navigational properties and references
    public virtual ActiveQuestionnaireResponseModel? ActiveQuestionnaireResponse { get; set; }
}
