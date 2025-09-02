using Database.Models;
using Database.Utils;
using Microsoft.EntityFrameworkCore;

namespace Database.Seeders;

public class DefaultQuestionnaireSeeder(ModelBuilder modelBuilder) : IDataSeeder<QuestionnaireTemplateModel>
{
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
