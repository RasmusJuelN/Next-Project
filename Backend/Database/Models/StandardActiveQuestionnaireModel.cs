
namespace Database.Models;

public class StandardActiveQuestionnaireModel : AnonymousActiveQuestionnaireModel
{
    /// <summary>
    /// Gets or sets the foreign key reference to the student assigned to complete this questionnaire.
    /// </summary>
    /// <remarks>
    /// Links to the StudentModel table to identify which student is responsible for completing this questionnaire.
    /// </remarks>
    [Required]
    public int StudentFK { get; set; }
    
    /// <summary>
    /// Gets or sets the foreign key reference to the teacher assigned to complete this questionnaire.
    /// </summary>
    /// <remarks>
    /// Links to the TeacherModel table to identify which teacher is responsible for completing this questionnaire.
    /// </remarks>
    [Required]
    public int TeacherFK { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the student completed their portion of the questionnaire.
    /// </summary>
    /// <remarks>
    /// Null indicates the student has not yet completed their responses.
    /// Once set, this timestamp indicates the student has submitted all required answers.
    /// </remarks>
    public DateTime? StudentCompletedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when the teacher completed their portion of the questionnaire review.
    /// </summary>
    /// <remarks>
    /// Null indicates the teacher has not yet completed their responses.
    /// Once set, this timestamp indicates the teacher has submitted all required answers.
    /// </remarks>
    public DateTime? TeacherCompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the student assigned to this questionnaire.
    /// </summary>
    /// <remarks>
    /// Provides access to the complete student information.
    /// </remarks>
    [Required]
    [ForeignKey(nameof(StudentFK))]
    public StudentModel? Student { get; set; }
    
    /// <summary>
    /// Gets or sets the navigation property to the teacher assigned to this questionnaire.
    /// </summary>
    /// <remarks>
    /// Provides access to the complete teacher information.
    /// </remarks>
    [Required]
    [ForeignKey(nameof(TeacherFK))]
    public TeacherModel? Teacher { get; set; }

    /// <summary>
    /// Gets or sets the collection of student responses submitted for this questionnaire.
    /// </summary>
    /// <remarks>
    /// Contains all answers provided by the student for the questionnaire questions.
    /// Virtual property enables lazy loading of student responses when needed.
    /// </remarks>
    public virtual ICollection<ActiveQuestionnaireStudentResponseModel> StudentAnswers { get; set; } = [];
    
    /// <summary>
    /// Gets or sets the collection of teacher responses submitted for this questionnaire.
    /// </summary>
    /// <remarks>
    /// Contains all answers provided by the teacher.
    /// Virtual property enables lazy loading of teacher responses when needed.
    /// </remarks>
    public virtual ICollection<ActiveQuestionnaireTeacherResponseModel> TeacherAnswers { get; set; } = [];
}
