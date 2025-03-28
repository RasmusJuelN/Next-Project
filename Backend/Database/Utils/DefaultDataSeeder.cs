using Database.Models;
using Newtonsoft.Json;

namespace Database.Utils;

static public class DefaultDataSeeder
{
    static public QuestionnaireTemplateModel? SeedQuestionnaireTemplate()
    {
        QuestionnaireTemplateModel? questionnaireTemplate;

        string json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Default/default_questionnaire.json"));
        questionnaireTemplate = JsonConvert.DeserializeObject<QuestionnaireTemplateModel>(json);

        return questionnaireTemplate;
    }
}
