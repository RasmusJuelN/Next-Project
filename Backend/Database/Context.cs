using Database.Models;
using Database.Utils;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class Context : DbContext
{
    public Context() {}
    public Context(DbContextOptions dbContextOptions) : base(dbContextOptions) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // QuestionnaireTemplate
        modelBuilder.Entity<QuestionnaireTemplateModel>(e => {
            e.Property(q => q.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(q => q.LastUpated).HasDefaultValueSql("SYSUTCDATETIME()");
        });
        
        // ActiveQuestionnaireModel
        modelBuilder.Entity<ActiveQuestionnaireModel>(e => {
            e.Property(a => a.ActivatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasOne(a => a.Student)
            .WithMany(u => u.ActiveQuestionnaires)
            .OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.Teacher)
            .WithMany(u => u.ActiveQuestionnaires)
            .OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.QuestionnaireTemplate)
            .WithMany(t => t.ActiveQuestionnaires)
            .OnDelete(DeleteBehavior.NoAction);
            e.HasMany(a => a.StudentAnswers)
            .WithOne(r => r.ActiveQuestionnaire)
            .OnDelete(DeleteBehavior.NoAction);
            e.HasMany(a => a.TeacherAnswers)
            .WithOne(r => r.ActiveQuestionnaire)
            .OnDelete(DeleteBehavior.NoAction);
        });

        // UserModel
        modelBuilder.Entity<UserBaseModel>(e => {
            e.Property(u => u.PrimaryRole)
            .HasConversion<string>();
        });

        // RevokedRefreshTokenModel
        modelBuilder.Entity<TrackedRefreshTokenModel>(e => {
            e.Property(r => r.ValidFrom)
            .HasDefaultValueSql("SYSUTCDATETIME()");
        });

        // ApplicationLogsModel
        modelBuilder.Entity<ApplicationLogsModel>(e => {
            e.Property(a => a.Timestamp)
            .HasDefaultValueSql("SYSUTCDATETIME()");
        });

        QuestionnaireTemplateModel? defaultTemplate = DefaultDataSeeder.SeedQuestionnaireTemplate();

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

        base.OnModelCreating(modelBuilder);
    }

    internal DbSet<QuestionnaireTemplateModel> QuestionnaireTemplates { get; set; }
    internal DbSet<QuestionnaireQuestionModel> QuestionnaireQuestions { get; set; }
    internal DbSet<QuestionnaireOptionModel> QuestionnaireOptions { get; set; }
    internal DbSet<ActiveQuestionnaireModel> ActiveQuestionnaires { get; set; }
    internal DbSet<ActiveQuestionnaireResponseBaseModel> ActiveQuestionnaireResponses { get; set; }
    internal DbSet<ActiveQuestionnaireStudentResponseModel> ActiveQuestionnaireStudentResponses { get; set; }
    internal DbSet<ActiveQuestionnaireTeacherResponseModel> ActiveQuestionnaireTeacherResponses { get; set; }
    internal DbSet<UserBaseModel> Users { get; set; }
    internal DbSet<StudentModel> Students { get; set; }
    internal DbSet<TeacherModel> Teachers { get; set; }
    internal DbSet<TrackedRefreshTokenModel> RevokedRefreshTokens { get; set; }
    internal DbSet<ApplicationLogsModel> ApplicationLogs { get; set; }
}