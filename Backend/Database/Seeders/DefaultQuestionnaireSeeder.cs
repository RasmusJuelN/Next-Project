using Database.Models;
using Database.Utils;
using Microsoft.EntityFrameworkCore;

namespace Database.Seeders;

public class DefaultQuestionnaireSeeder(ModelBuilder modelBuilder) : IDataSeeder<QuestionnaireTemplateModel>
{
    /// <summary>
    /// Initializes the default questionnaire template data by seeding the database with predefined questionnaire data.
    /// </summary>
    /// <remarks>
    /// This method loads a default questionnaire template from a JSON file and seeds the database with the template data.
    /// Due to Entity Framework seeding limitations with relationships, the method first seeds child entities (options and questions)
    /// individually, then seeds the parent template entity with foreign key references established.
    /// <para>The seeding process follows this order:</para>
    /// <list type="bullet">
    /// <item><description>Seeds questionnaire options for each question</description></item>
    /// <item><description>Seeds questionnaire questions</description></item>
    /// <item><description>Seeds the questionnaire template</description></item>
    /// </list>
    /// <para>All relationship collections are cleared from entities before seeding to avoid EF conflicts.</para>
    /// </remarks>
    public void InitializeData()
    {
        QuestionnaireTemplateModel? defaultTemplate = IDataSeeder<QuestionnaireTemplateModel>.LoadSeed("Default/default_questionnaire.json");

        if (defaultTemplate is not null)
        {
            // Seeding doesn't allow relationships in the entity
            // so we first seed each entity in the relationships
            // and link them by their foreign keys, and then remove
            // them from the entity.
            foreach (QuestionnaireQuestionModel question in defaultTemplate.Questions)
            {
                modelBuilder.Entity<QuestionnaireOptionModel>().HasData(question.Options);
                question.Options = [];
            }
            
            modelBuilder.Entity<QuestionnaireQuestionModel>().HasData(defaultTemplate.Questions);

            defaultTemplate.Questions = [];

            modelBuilder.Entity<QuestionnaireTemplateModel>().HasData(defaultTemplate);
        }
    }
}
