using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class Context(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
{
    private readonly DbContextOptions _dbContextOptions = dbContextOptions;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // QuestionnaireTemplate
        modelBuilder.Entity<QuestionnaireTemplate>()
            .HasKey(q => q.Id);
        modelBuilder.Entity<QuestionnaireTemplate>()
            .HasIndex(q => q.TemplateTitle)
            .IsUnique();
        modelBuilder.Entity<QuestionnaireTemplate>()
            .Property(e => e.TemplateTitle)
            .HasMaxLength(150);
        modelBuilder.Entity<QuestionnaireTemplate>()
            .HasMany(e => e.Questions)
            .WithOne(e => e.QuestionnaireTemplate)
            .HasForeignKey(e => e.QuestionnaireTemplateId)
            .HasPrincipalKey(e => e.Id);
        modelBuilder.Entity<QuestionnaireTemplate>()
            .HasMany(e => e.ActiveQuestionnaires)
            .WithOne(e => e.QuestionnaireTemplate)
            .HasForeignKey(e => e.TemplateId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired(false);
        modelBuilder.Entity<QuestionnaireTemplate>()
            .Property(q => q.CreatedAt)
            .HasDefaultValueSql("getdate()");
        
        // QuestionnaireQuestion
        modelBuilder.Entity<QuestionnaireQuestion>()
            .HasKey(e => e.Id);
        modelBuilder.Entity<QuestionnaireQuestion>()
            .HasMany(e => e.Options)
            .WithOne(e => e.Question)
            .HasForeignKey(e => e.QuestionId)
            .HasPrincipalKey(e => e.Id);
        modelBuilder.Entity<QuestionnaireQuestion>()
            .HasOne(e => e.QuestionnaireTemplate)
            .WithMany(e => e.Questions);
        modelBuilder.Entity<QuestionnaireQuestion>()
            .Property(e => e.QuestionTitle)
            .HasMaxLength(100);
        
        // QuestionnaireOption
        modelBuilder.Entity<QuestionnaireOption>()
            .HasKey(e => e.Id);
        modelBuilder.Entity<QuestionnaireOption>()
            .Property(e => e.OptionText)
            .HasMaxLength(500);
        modelBuilder.Entity<QuestionnaireOption>()
            .HasOne(e => e.Question)
            .WithMany(e => e.Options);
        
        // ActiveQuestionnaireModel
        modelBuilder.Entity<ActiveQuestionnaire>()
            .HasKey(e => e.Id);
        modelBuilder.Entity<ActiveQuestionnaire>()
            .Property(e => e.ActivatedAt)
            .HasDefaultValueSql("getdate()");
        modelBuilder.Entity<ActiveQuestionnaire>()
            .Property(e => e.StudentCompletedAt)
            .IsRequired(false);
        modelBuilder.Entity<ActiveQuestionnaire>()
            .Property(e => e.TeacherCompletedAt)
            .IsRequired(false);
        modelBuilder.Entity<ActiveQuestionnaire>()
            .HasOne(e => e.Student)
            .WithMany(e => e.StudentQuestionnaires)
            .HasForeignKey(e => e.StudentId);
        modelBuilder.Entity<ActiveQuestionnaire>()
            .HasOne(e => e.Teacher)
            .WithMany(e => e.TeacherQuestionnaires)
            .HasForeignKey(e => e.TeacherId);
        modelBuilder.Entity<ActiveQuestionnaire>()
            .HasOne(e => e.QuestionnaireTemplate)
            .WithMany(e => e.ActiveQuestionnaires);
        modelBuilder.Entity<ActiveQuestionnaire>()
            .HasMany(e => e.Answers)
            .WithOne(e => e.ActiveQuestionnaire)
            .HasForeignKey(e => e.ActiveQuestionnaireId)
            .HasPrincipalKey(e => e.Id);
        
        // ActiveQuestionnaireAnswerModel
        modelBuilder.Entity<ActiveQuestionnaireAnswerModel>()
            .HasKey(e => e.Id);
        modelBuilder.Entity<ActiveQuestionnaireAnswerModel>()
            .Property(e => e.CustomAnswerText)
            .IsRequired(false)
            .HasMaxLength(500);
        modelBuilder.Entity<ActiveQuestionnaireAnswerModel>()
            .HasOne(e => e.ActiveQuestionnaire)
            .WithMany(e => e.Answers);
        
        // UserModel
        modelBuilder.Entity<UserModel>()
            .HasKey(e => e.Id);
        modelBuilder.Entity<UserModel>()
            .Property(e => e.UserName)
            .HasMaxLength(100);
        modelBuilder.Entity<UserModel>()
            .Property(e => e.FullName)
            .HasMaxLength(150);
        modelBuilder.Entity<UserModel>()
            .Property(e => e.Role)
            .HasConversion<string>();
        modelBuilder.Entity<UserModel>()
            .HasMany(e => e.StudentQuestionnaires)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired(false);
        modelBuilder.Entity<UserModel>()
            .HasMany(e => e.TeacherQuestionnaires)
            .WithOne(e => e.Teacher)
            .HasForeignKey(e => e.TeacherId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired(false);
        modelBuilder.Entity<UserModel>()
            .ToTable(e => e.HasCheckConstraint("CK_ActiveQuestionnaires", "(StudentQuestionnaires IS NOT NULL OR TeacherQuestionnaires IS NOT NULL)"));


        base.OnModelCreating(modelBuilder);
    }
}