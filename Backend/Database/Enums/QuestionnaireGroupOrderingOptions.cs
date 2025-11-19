using Database.Attributes;
using Database.Extensions;
using System.Text.Json.Serialization;

namespace Database.Enums
{
    /// <summary>
    /// Defines the available ordering options for querying questionnaire groups.
    /// </summary>
    /// <remarks>
    /// Each enum value is decorated with a <see cref="QueryMethodAttribute"/> that
    /// maps to the corresponding ordering extension method for LINQ queries.
    /// </remarks>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum QuestionnaireGroupOrderingOptions
    {
        /// <summary>
        /// Orders groups alphabetically by name in ascending order (A–Z).
        /// </summary>
        [QueryMethod(nameof(IQueryableExtensions.OrderByNameAsc))]
        NameAsc,

        /// <summary>
        /// Orders groups alphabetically by name in descending order (Z–A).
        /// </summary>
        [QueryMethod(nameof(IQueryableExtensions.OrderByNameDesc))]
        NameDesc,

        /// <summary>
        /// Orders groups by creation date in ascending order (oldest first).
        /// </summary>
        [QueryMethod(nameof(IQueryableExtensions.OrderByCreatedAtAsc))]
        CreatedAtAsc,

        /// <summary>
        /// Orders groups by creation date in descending order (newest first).
        /// </summary>
        [QueryMethod(nameof(IQueryableExtensions.OrderByCreatedAtDesc))]
        CreatedAtDesc
    }
}