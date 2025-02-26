using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Database;

public class Context : DbContext
{
    public Context() {}
    public Context(DbContextOptions dbContextOptions) : base(dbContextOptions) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // QuestionnaireTemplate
        modelBuilder.Entity<QuestionnaireTemplateModel>(e => {
            e.Property(q => q.CreatedAt).HasDefaultValueSql("getdate()");
            e.Property(q => q.LastUpated).HasDefaultValueSql("getdate()");
        });
        
        // ActiveQuestionnaireModel
        modelBuilder.Entity<ActiveQuestionnaireModel>(e => {
            e.Property(a => a.ActivatedAt)
            .HasDefaultValueSql("getdate()");
            e.HasOne(a => a.Student)
            .WithMany(u => u.ActiveQuestionnaires)
            .OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.Teacher)
            .WithMany(u => u.ActiveQuestionnaires)
            .OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.QuestionnaireTemplate)
            .WithMany(t => t.ActiveQuestionnaires)
            .OnDelete(DeleteBehavior.NoAction);
        });

        // ActiveQuestionnaireResponseModel
        modelBuilder.Entity<ActiveQuestionnaireResponseModel>(e => {
            e.HasOne(r => r.ActiveQuestionnaire)
            .WithMany(a => a.Answers)
            .OnDelete(DeleteBehavior.NoAction);
        });

        // UserModel
        modelBuilder.Entity<UserBaseModel>(e => {
            e.Property(u => u.PrimaryRole)
            .HasConversion<string>();
        });

        // RevokedRefreshTokenModel
        modelBuilder.Entity<TrackedRefreshTokenModel>(e => {
            e.Property(r => r.CreatedAt)
            .HasDefaultValueSql("getdate()");
        });

        // ApplicationLogsModel
        modelBuilder.Entity<ApplicationLogsModel>(e => {
            e.Property(a => a.Timestamp)
            .HasDefaultValueSql("getdate()");
        });

        base.OnModelCreating(modelBuilder);
    }

    internal DbSet<QuestionnaireTemplateModel> QuestionnaireTemplates { get; set; }
    internal DbSet<QuestionnaireQuestionModel> QuestionnaireQuestions { get; set; }
    internal DbSet<QuestionnaireOptionModel> QuestionnaireOptions { get; set; }
    internal DbSet<ActiveQuestionnaireModel> ActiveQuestionnaires { get; set; }
    internal DbSet<ActiveQuestionnaireQuestionModel> ActiveQuestionnaireQuestions { get; set; }
    internal DbSet<ActiveQuestionnaireOptionModel> ActiveQuestionnaireOptions { get; set; }
    internal DbSet<ActiveQuestionnaireResponseModel> ActiveQuestionnaireResponses { get; set; }
    internal DbSet<CustomAnswerModelBase> CustomAnswers { get; set; }
    internal DbSet<StudentCustomAnswerModel> StudentCustomAnswers { get; set; }
    internal DbSet<TeacherCustomAnswerModel> TeacherCustomAnswers { get; set; }
    internal DbSet<UserBaseModel> Users { get; set; }
    internal DbSet<StudentModel> Students { get; set; }
    internal DbSet<TeacherModel> Teachers { get; set; }
    internal DbSet<TrackedRefreshTokenModel> RevokedRefreshTokens { get; set; }
    internal DbSet<ApplicationLogsModel> ApplicationLogs { get; set; }
}