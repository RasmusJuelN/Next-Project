
namespace Database.Models;

/// <summary>
/// Represents an active questionnaire instance that has been activated from a template and assigned to specific student-teacher pairs.
/// This model tracks the lifecycle of a questionnaire from activation through completion by both parties.
/// </summary>
/// <remarks>
/// Active questionnaires are created when a questionnaire template is activated for a specific student-teacher combination.
/// The model tracks completion status separately for students and teachers, allowing for asynchronous completion workflows.
/// Indexed on Title for efficient searching and filtering operations.
/// </remarks>
[Table("ActiveQuestionnaire")]
[Index(nameof(Title))]
public class ActiveQuestionnaireModel
{
    /// <summary>
    /// Gets or sets the unique identifier for this active questionnaire instance.
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the title of the questionnaire, copied from the template at activation time.
    /// </summary>
    /// <remarks>
    /// This field is indexed for efficient searching.
    /// </remarks>
    [Required]
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the optional description of the questionnaire, copied from the template at activation time.
    /// </summary>
    /// <remarks>
    /// Provides additional context about the questionnaire purpose and content.
    /// </remarks>
    public string? Description { get; set; }
    
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
    /// Gets or sets the foreign key reference to the questionnaire template this active instance was created from.
    /// </summary>
    /// <remarks>
    /// Links to the QuestionnaireTemplateModel to maintain the relationship with the original template.
    /// This relationship is preserved even if the template is later modified.
    /// </remarks>
    [Required]
    public Guid QuestionnaireTemplateFK { get; set; }

    [Required]
    public Guid GroupId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this questionnaire was activated and made available for completion.
    /// </summary>
    /// <remarks>
    /// Default value is configured in Fluent API to use the current timestamp at creation.
    /// Used for tracking questionnaire age and ordering by activation time.
    /// </remarks>
    [Required]
    public DateTime ActivatedAt { get; set; }
    
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
    /// Gets or sets the navigation property to the questionnaire template this instance was created from.
    /// </summary>
    /// <remarks>
    /// Provides access to the original template structure including questions and options.
    /// Useful for referencing the template structure during questionnaire processing.
    /// </remarks>
    [Required]
    [ForeignKey(nameof(QuestionnaireTemplateFK))]
    public QuestionnaireTemplateModel? QuestionnaireTemplate { get; set; }

    [ForeignKey(nameof(GroupId))]
    public QuestionnaireGroupModel? Group { get; set; }

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
