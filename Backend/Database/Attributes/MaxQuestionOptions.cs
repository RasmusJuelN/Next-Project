using System.ComponentModel.DataAnnotations;

namespace API.Attributes;

/// <summary>
/// Validation attribute that ensures a collection of questionnaire options does not exceed the maximum allowed count.
/// </summary>
/// <remarks>
/// This attribute validates that the number of options in a questionnaire question does not exceed 10,
/// which is the business rule limit for questionnaire options.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class MaxQuestionOptionsAttribute : ValidationAttribute
{
    private const int MaxOptionsCount = 10;

    /// <summary>
    /// Initializes a new instance of the MaxQuestionOptionsAttribute class.
    /// </summary>
    public MaxQuestionOptionsAttribute() : base($"The maximum number of options allowed is {MaxOptionsCount}.")
    {
    }

    /// <summary>
    /// Validates that the collection does not exceed the maximum options count.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>ValidationResult.Success if valid, otherwise a ValidationResult with an error message.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success; // Null is considered valid (let Required attribute handle null checks)
        }

        if (value is not System.Collections.IEnumerable enumerable)
        {
            return new ValidationResult("Value must be a collection.");
        }

        var count = 0;
        foreach (var _ in enumerable)
        {
            count++;
            if (count > MaxOptionsCount)
            {
                return new ValidationResult(
                    $"The number of options ({count}) exceeds the maximum allowed limit of {MaxOptionsCount}.",
                    validationContext.MemberName != null ? new[] { validationContext.MemberName } : null);
            }
        }

        return ValidationResult.Success;
    }
}