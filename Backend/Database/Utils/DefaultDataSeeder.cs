using Database.Models;
using Newtonsoft.Json;

namespace Database.Utils;

/// <summary>
/// Provides functionality to seed default data into the database.
/// </summary>
static public class DefaultDataSeeder
{
    /// <summary>
    /// Seeds a default questionnaire template by reading and deserializing data from a JSON file.
    /// </summary>
    /// <returns>
    /// A <see cref="QuestionnaireTemplateModel"/> object containing the default questionnaire template data,
    /// or null if the deserialization fails or the file cannot be read.
    /// </returns>
    /// <remarks>
    /// This method reads the JSON file located at "Default/default_questionnaire.json" relative to the application's base directory
    /// and deserializes it into a QuestionnaireTemplateModel object using Newtonsoft.Json.
    /// </remarks>
    static public QuestionnaireTemplateModel? SeedQuestionnaireTemplate()
    {
        QuestionnaireTemplateModel? questionnaireTemplate;

        string json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Default/default_questionnaire.json"));
        questionnaireTemplate = JsonConvert.DeserializeObject<QuestionnaireTemplateModel>(json);

        return questionnaireTemplate;
    }
}
