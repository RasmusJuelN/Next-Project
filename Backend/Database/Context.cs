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

        modelBuilder.Entity<QuestionnaireGroupModel>(e => {
            e.HasMany(g => g.Questionnaires)
            .WithOne(q => q.Group)
            .HasForeignKey(q => q.GroupId)
            .OnDelete(DeleteBehavior.Cascade); // Or NoAction, as you prefer
        });
        
        // QuestionnaireTemplate
        modelBuilder.Entity<QuestionnaireTemplateModel>(e => {
            e.Property(q => q.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(q => q.LastUpated).HasDefaultValueSql("SYSUTCDATETIME()");
            
            // Delete Questions linked to a certain questionnaire
            e.HasMany(q => q.Questions)
            .WithOne(q => q.QuestionnaireTemplate)
            .HasPrincipalKey(q => q.Id)
            .HasForeignKey(q => q.QuestionnaireTemplateFK)
            .OnDelete(DeleteBehavior.Cascade);
            
            // Delete active questionnaires that reference the deleted questionnaire
            e.HasMany(q => q.ActiveQuestionnaires)
            .WithOne(a => a.QuestionnaireTemplate)
            .HasPrincipalKey(q => q.Id)
            .HasForeignKey(a => a.QuestionnaireTemplateFK)
            .OnDelete(DeleteBehavior.Cascade);
        });

        // QuestionnaireTemplateQuestion
        modelBuilder.Entity<QuestionnaireQuestionModel>(e => {
            e.HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasPrincipalKey(q => q.Id)
            .HasForeignKey(o => o.QuestionFK)
            .OnDelete(DeleteBehavior.Cascade);
        });
        
        // ActiveQuestionnaireModel
        modelBuilder.Entity<ActiveQuestionnaireModel>(e => {
            e.Property(a => a.ActivatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");
            
            // We don't wanna delete users since they might have other active questionnaires assigned
            e.HasOne(a => a.Student)
            .WithMany(u => u.ActiveQuestionnaires)
            .HasPrincipalKey(u => u.Id)
            .HasForeignKey(a => a.StudentFK)
            .OnDelete(DeleteBehavior.NoAction);
            e.HasOne(a => a.Teacher)
            .WithMany(u => u.ActiveQuestionnaires)
            .HasPrincipalKey(u => u.Id)
            .HasForeignKey(a => a.TeacherFK)
            .OnDelete(DeleteBehavior.NoAction);
            
            // Deleting an active questionnaire shouldn't delete the template
            e.HasOne(a => a.QuestionnaireTemplate)
            .WithMany(t => t.ActiveQuestionnaires)
            .HasPrincipalKey(t => t.Id)
            .HasForeignKey(a => a.QuestionnaireTemplateFK)
            .OnDelete(DeleteBehavior.NoAction);
            
            // No reason to keep the answers if we're deleting the active questionnaire
            e.HasMany(a => a.StudentAnswers)
            .WithOne(r => r.ActiveQuestionnaire)
            .HasPrincipalKey(a => a.Id)
            .HasForeignKey(r => r.ActiveQuestionnaireFK)
            .OnDelete(DeleteBehavior.Cascade);
            e.HasMany(a => a.TeacherAnswers)
            .WithOne(r => r.ActiveQuestionnaire)
            .HasPrincipalKey(a => a.Id)
            .HasForeignKey(r => r.ActiveQuestionnaireFK)
            .OnDelete(DeleteBehavior.Cascade);
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

        // Initialize all data seeders
        SeederHelper seederHelper = new(modelBuilder);
        seederHelper.Seed();

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
    internal DbSet<QuestionnaireGroupModel> QuestionnaireGroups { get; set; }

}