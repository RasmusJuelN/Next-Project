using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class Context(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // QuestionnaireTemplate
        modelBuilder.Entity<QuestionnaireTemplateModel>(e => {
            e.ToTable("QuestionnaireTemplate");
            e.HasKey(q => q.Id);
            e.HasIndex(q => q.TemplateTitle).IsUnique();
            e.Property(q => q.TemplateTitle).HasMaxLength(150);
            e.HasMany(q => q.Questions)
            .WithOne(q => q.QuestionnaireTemplate)
            .HasForeignKey(q => q.QuestionnaireTemplateId)
            .HasPrincipalKey(q => q.Id);
            e.HasMany(q => q.ActiveQuestionnaires)
            .WithOne(q => q.QuestionnaireTemplate)
            .HasForeignKey(q => q.QuestionnaireTemplateId)
            .HasPrincipalKey(q => q.Id)
            .IsRequired(false);
            e.Property(q => q.CreatedAt).HasDefaultValueSql("getdate()");
        });
        
        // QuestionnaireQuestion
        modelBuilder.Entity<QuestionnaireQuestionModel>(e => {
            e.ToTable("QuestionnaireTemplateQuestion");
            e.HasKey(q => q.Id);
            e.HasMany(q => q.Options)
            .WithOne(q => q.Question)
            .HasForeignKey(q => q.QuestionId)
            .HasPrincipalKey(q => q.Id);
            e.HasOne(q => q.QuestionnaireTemplate).WithMany(q => q.Questions);
            e.Property(q => q.Prompt)
            .HasMaxLength(500);
        });
        
        // QuestionnaireOption
        modelBuilder.Entity<QuestionnaireOptionModel>(e => {
            e.ToTable("QuestionnaireTemplateOption");
            e.HasKey(q => q.Id);
            e.Property(q => q.DisplayText)
            .HasMaxLength(150);
            e.HasOne(q => q.Question).WithMany(q => q.Options);
        });
        
        // ActiveQuestionnaireModel
        modelBuilder.Entity<ActiveQuestionnaireModel>(e => {
            e.ToTable("ActiveQuestionnaire");
            e.HasKey(a => a.Id);
            e.Property(a => a.ActivatedAt)
            .HasDefaultValueSql("getdate()");
            e.Property(a => a.StudentCompletedAt)
            .IsRequired(false);
            e.Property(a => a.TeacherCompletedAt)
            .IsRequired(false);
            e.HasOne(a => a.Student)
            .WithMany()
            .HasForeignKey(a => a.StudentId)
            .HasPrincipalKey(u => u.Id);
            e.HasOne(a => a.Teacher)
            .WithMany()
            .HasForeignKey(a => a.TeacherId)
            .HasPrincipalKey(u => u.Id);
            e.HasOne(a => a.QuestionnaireTemplate)
            .WithMany(q => q.ActiveQuestionnaires);
            e.HasMany(a => a.Answers)
            .WithOne(a => a.ActiveQuestionnaire)
            .HasForeignKey(a => a.ActiveQuestionnaireId)
            .HasPrincipalKey(a => a.Id);
        });

        // ActiveQuestionnaireQuestionModel
        modelBuilder.Entity<ActiveQuestionnaireQuestionModel>(e => {
            e.ToTable("ActiveQuestionnaireQuestion");
            e.HasKey(a => a.Id);
            e.HasMany(a => a.ActiveQuestionnaireOptions)
            .WithOne(a => a.ActiveQuestionnaireQuestion)
            .HasForeignKey(a => a.ActiveQuestionnaireQuestionId)
            .HasPrincipalKey(a => a.Id);
            e.HasOne(a => a.ActiveQuestionnaire).WithMany(a => a.ActiveQuestionnaireQuestions);
            e.Property(a => a.Prompt)
            .HasMaxLength(500);
        });

        // ActiveQuestionnaireOptionModel
        modelBuilder.Entity<ActiveQuestionnaireOptionModel>(e => {
            e.ToTable("ActiveQuestionnaireOption");
            e.HasKey(a => a.Id);
            e.Property(a => a.DisplayText)
            .HasMaxLength(150);
            e.HasOne(a => a.ActiveQuestionnaireQuestion)
            .WithMany(a => a.ActiveQuestionnaireOptions);
        });
        
        // ActiveQuestionnaireResponseModel
        modelBuilder.Entity<ActiveQuestionnaireResponseModel>(e => {
            e.ToTable("ActiveQuestionnaireResponse");
            e.HasKey(a => a.Id);
            e.Property(a => a.StudentResponse)
            .IsRequired(false);
            e.Property(a => a.TeacherResponse)
            .IsRequired(false);
            e.HasOne(a => a.ActiveQuestionnaire)
            .WithMany(a => a.Answers);
            e.HasOne(a => a.CustomStudentResponse)
            .WithOne()
            .HasForeignKey<ActiveQuestionnaireResponseModel>(a => a.CustomStudentResponseId)
            .HasPrincipalKey<CustomAnswerModel>(c => c.Id)
            .IsRequired(false);
            e.HasOne(a => a.CustomTeacherResponse)
            .WithOne()
            .HasForeignKey<ActiveQuestionnaireResponseModel>(a => a.CustomTeacherResponseId)
            .HasPrincipalKey<CustomAnswerModel>(c => c.Id)
            .IsRequired(false);
        });

        // CustomAnswerModel
        modelBuilder.Entity<CustomAnswerModel>(e => {
            e.ToTable("CustomAnswer");
            e.HasKey(c => c.Id);
            e.Property(c => c.Response)
            .HasMaxLength(500);
            e.HasOne(c => c.ActiveQuestionnaireResponse)
            .WithOne()
            .HasForeignKey<CustomAnswerModel>(c => c.ActiveQuestionnaireResponseId)
            .HasPrincipalKey<ActiveQuestionnaireResponseModel>(a => a.Id);
        });
        
        // UserModel
        modelBuilder.Entity<UserModel>(e => {
            e.ToTable("User");
            e.HasKey(u => u.Id);
            e.Property(u => u.UserName)
            .HasMaxLength(100);
            e.HasIndex(u => u.UserName)
            .IsUnique();
            e.Property(u => u.FullName)
            .HasMaxLength(150);
            e.Property(u => u.PrimaryRole)
            .HasConversion<string>();
            e.HasMany(u => u.ActiveQuestionnaires)
            .WithOne()
            .HasForeignKey(a => a.Id)
            .HasPrincipalKey(u => u.Id);
        });

        base.OnModelCreating(modelBuilder);
    }
}