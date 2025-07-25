﻿// <auto-generated />
using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Database.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Database.Models.ActiveQuestionnaireModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ActivatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("QuestionnaireTemplateFK")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("StudentCompletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("StudentFK")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TeacherCompletedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("TeacherFK")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("QuestionnaireTemplateFK");

                    b.HasIndex("StudentFK");

                    b.HasIndex("TeacherFK");

                    b.HasIndex("Title");

                    b.ToTable("ActiveQuestionnaire");
                });

            modelBuilder.Entity("Database.Models.ActiveQuestionnaireResponseBaseModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<Guid>("ActiveQuestionnaireFK")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CustomResponse")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(55)
                        .HasColumnType("nvarchar(55)");

                    b.Property<int?>("OptionFK")
                        .HasColumnType("int");

                    b.Property<int>("QuestionFK")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OptionFK");

                    b.HasIndex("QuestionFK");

                    b.ToTable("ActiveQuestionnaireResponse");

                    b.HasDiscriminator().HasValue("ActiveQuestionnaireResponseBaseModel");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Database.Models.ApplicationLogsModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<string>("Exception")
                        .HasMaxLength(5000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LogLevel")
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.HasKey("Id");

                    b.ToTable("ApplicationLogs");
                });

            modelBuilder.Entity("Database.Models.QuestionnaireOptionModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DisplayText")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("OptionValue")
                        .HasColumnType("int");

                    b.Property<int>("QuestionFK")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("QuestionFK");

                    b.ToTable("QuestionnaireTemplateOption");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayText = "Viser lidt eller ingen forståelse for arbejdsopgaverne.",
                            OptionValue = 1,
                            QuestionFK = 1
                        },
                        new
                        {
                            Id = 2,
                            DisplayText = "Forstår arbejdsopgaverne, men kan ikke anvende den i praksis. Har svært ved at tilegne sig ny viden.",
                            OptionValue = 2,
                            QuestionFK = 1
                        },
                        new
                        {
                            Id = 3,
                            DisplayText = "Let ved at forstå arbejdsopgaverne og anvende den i praksis. Har let ved at tilegne sig ny viden.",
                            OptionValue = 3,
                            QuestionFK = 1
                        },
                        new
                        {
                            Id = 4,
                            DisplayText = "Mindre behov for oplæring end normalt. Kan selv finde/tilegne sig ny viden.",
                            OptionValue = 4,
                            QuestionFK = 1
                        },
                        new
                        {
                            Id = 5,
                            DisplayText = "Behøver næsten ingen oplæring. Kan ved selvstudium, endog ved svært tilgængeligt materiale, tilegne sig ny viden.",
                            OptionValue = 5,
                            QuestionFK = 1
                        },
                        new
                        {
                            Id = 6,
                            DisplayText = "Viser intet initiativ. Er passiv, uinteresseret og uselvstændig.",
                            OptionValue = 1,
                            QuestionFK = 2
                        },
                        new
                        {
                            Id = 7,
                            DisplayText = "Viser ringe initiativ. Kommer ikke selv med løsningsforslag. Viser ingen interesse i at tilrettelægge eget arbejde.",
                            OptionValue = 2,
                            QuestionFK = 2
                        },
                        new
                        {
                            Id = 8,
                            DisplayText = "Viser normalt initiativ. Kommer selv med løsningsforslag. Tilrettelægger eget arbejde.",
                            OptionValue = 3,
                            QuestionFK = 2
                        },
                        new
                        {
                            Id = 9,
                            DisplayText = "Meget initiativrig. Kommer selv med løsningsforslag. Gode evner for at tilrettelægge eget og andres arbejde.",
                            OptionValue = 4,
                            QuestionFK = 2
                        },
                        new
                        {
                            Id = 10,
                            DisplayText = "Overordentlig initiativrig. Løser selv problemerne. Tilrettelægger selvstændigt arbejdet for mig selv og andre.",
                            OptionValue = 5,
                            QuestionFK = 2
                        },
                        new
                        {
                            Id = 11,
                            DisplayText = "Uacceptabel",
                            OptionValue = 1,
                            QuestionFK = 3
                        },
                        new
                        {
                            Id = 12,
                            DisplayText = "Under middel",
                            OptionValue = 2,
                            QuestionFK = 3
                        },
                        new
                        {
                            Id = 13,
                            DisplayText = "Middel",
                            OptionValue = 3,
                            QuestionFK = 3
                        },
                        new
                        {
                            Id = 14,
                            DisplayText = "Over middel",
                            OptionValue = 4,
                            QuestionFK = 3
                        },
                        new
                        {
                            Id = 15,
                            DisplayText = "Særdeles god",
                            OptionValue = 5,
                            QuestionFK = 3
                        },
                        new
                        {
                            Id = 16,
                            DisplayText = "Omgås materialer, maskiner og værktøj på en sløset og ligegyldig måde. Holder ikke sin arbejdsplads ordentlig.",
                            OptionValue = 1,
                            QuestionFK = 4
                        },
                        new
                        {
                            Id = 17,
                            DisplayText = "Bruger maskiner og værktøj uden megen omtanke. Mindre god orden og omhyggelighed.",
                            OptionValue = 2,
                            QuestionFK = 4
                        },
                        new
                        {
                            Id = 18,
                            DisplayText = "Påpasselighed og omhyggelighed middel. Rimelig god orden.",
                            OptionValue = 3,
                            QuestionFK = 4
                        },
                        new
                        {
                            Id = 19,
                            DisplayText = "Meget påpasselig både i praktik og teori. God orden.",
                            OptionValue = 4,
                            QuestionFK = 4
                        },
                        new
                        {
                            Id = 20,
                            DisplayText = "I høj grad påpasselig. God forståelse for materialevalg. Særdeles god orden.",
                            OptionValue = 5,
                            QuestionFK = 4
                        },
                        new
                        {
                            Id = 21,
                            DisplayText = "N/A",
                            OptionValue = 1,
                            QuestionFK = 5
                        },
                        new
                        {
                            Id = 22,
                            DisplayText = "N/A",
                            OptionValue = 1,
                            QuestionFK = 6
                        },
                        new
                        {
                            Id = 23,
                            DisplayText = "N/A",
                            OptionValue = 1,
                            QuestionFK = 7
                        },
                        new
                        {
                            Id = 24,
                            DisplayText = "Du møder ikke hver dag til tiden.",
                            OptionValue = 1,
                            QuestionFK = 8
                        },
                        new
                        {
                            Id = 25,
                            DisplayText = "Du møder næsten hver dag til tiden.",
                            OptionValue = 2,
                            QuestionFK = 8
                        },
                        new
                        {
                            Id = 26,
                            DisplayText = "Du møder hver dag til tiden.",
                            OptionValue = 3,
                            QuestionFK = 8
                        },
                        new
                        {
                            Id = 27,
                            DisplayText = "Du melder ikke afbud ved sygdom.",
                            OptionValue = 1,
                            QuestionFK = 9
                        },
                        new
                        {
                            Id = 28,
                            DisplayText = "Du melder, for det meste afbud, når du er syg.",
                            OptionValue = 2,
                            QuestionFK = 9
                        },
                        new
                        {
                            Id = 29,
                            DisplayText = "Du melder afbud, når du er syg.",
                            OptionValue = 3,
                            QuestionFK = 9
                        },
                        new
                        {
                            Id = 30,
                            DisplayText = "Du har et stort fravær.",
                            OptionValue = 1,
                            QuestionFK = 10
                        },
                        new
                        {
                            Id = 31,
                            DisplayText = "Du har noget fravær.",
                            OptionValue = 2,
                            QuestionFK = 10
                        },
                        new
                        {
                            Id = 32,
                            DisplayText = "Du har stort set ingen fravær.",
                            OptionValue = 3,
                            QuestionFK = 10
                        },
                        new
                        {
                            Id = 33,
                            DisplayText = "Du har ingen fravær.",
                            OptionValue = 4,
                            QuestionFK = 10
                        },
                        new
                        {
                            Id = 34,
                            DisplayText = "Du søger ingen praktikpladser.",
                            OptionValue = 1,
                            QuestionFK = 11
                        },
                        new
                        {
                            Id = 35,
                            DisplayText = "Du ved, at du skal søge alle relevante praktikpladser, men det kniber med handlingen.",
                            OptionValue = 2,
                            QuestionFK = 11
                        },
                        new
                        {
                            Id = 36,
                            DisplayText = "Du søger alle relevante praktikpladser, men skal have hjælp til at søge praktikpladser, der ligger længere væk end i din bopælskommune.",
                            OptionValue = 3,
                            QuestionFK = 11
                        },
                        new
                        {
                            Id = 37,
                            DisplayText = "Du søger alle relevante praktikpladser også dem der ligger uden for din bopælskommune.",
                            OptionValue = 4,
                            QuestionFK = 11
                        },
                        new
                        {
                            Id = 38,
                            DisplayText = "Du søger alle relevante praktikpladser også dem der ligger uden for din bopælskommune. Du søger også praktikplads inden for en anden uddannelse, som dit GF giver adgang til.",
                            OptionValue = 5,
                            QuestionFK = 11
                        },
                        new
                        {
                            Id = 39,
                            DisplayText = "Du har ikke en synlig profil på praktikpladsen.dk.",
                            OptionValue = 1,
                            QuestionFK = 12
                        },
                        new
                        {
                            Id = 40,
                            DisplayText = "Du skal ofte påmindes om at synliggøre din profil på praktikpladsen.dk.",
                            OptionValue = 2,
                            QuestionFK = 12
                        },
                        new
                        {
                            Id = 41,
                            DisplayText = "Du har altid en synlig, men ikke opdateret profil på praktikpladsen.dk.",
                            OptionValue = 3,
                            QuestionFK = 12
                        },
                        new
                        {
                            Id = 42,
                            DisplayText = "Du har altid en opdateret og synlig profil på praktikpladsen.dk.",
                            OptionValue = 4,
                            QuestionFK = 12
                        });
                });

            modelBuilder.Entity("Database.Models.QuestionnaireQuestionModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("AllowCustom")
                        .HasColumnType("bit");

                    b.Property<string>("Prompt")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<Guid>("QuestionnaireTemplateFK")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("QuestionnaireTemplateFK");

                    b.ToTable("QuestionnaireTemplateQuestion");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AllowCustom = false,
                            Prompt = "Indlæringsevne",
                            QuestionnaireTemplateFK = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6")
                        },
                        new
                        {
                            Id = 2,
                            AllowCustom = false,
                            Prompt = "Kreativitet og selvstændighed",
                            QuestionnaireTemplateFK = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6")
                        },
                        new
                        {
                            Id = 3,
                            AllowCustom = false,
                            Prompt = "Arbejdsindsats",
                            QuestionnaireTemplateFK = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6")
                        },
                        new
                        {
                            Id = 4,
                            AllowCustom = false,
                            Prompt = "Orden og omhyggelighed",
                            QuestionnaireTemplateFK = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6")
                        },
                        new
                        {
                            Id = 5,
                            AllowCustom = false,
                            Prompt = "N/A",
                            QuestionnaireTemplateFK = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6")
                        },
                        new
                        {
                            Id = 6,
                            AllowCustom = false,
                            Prompt = "N/A",
                            QuestionnaireTemplateFK = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6")
                        },
                        new
                        {
                            Id = 7,
                            AllowCustom = false,
                            Prompt = "N/A",
                            QuestionnaireTemplateFK = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6")
                        },
                        new
                        {
                            Id = 8,
                            AllowCustom = false,
                            Prompt = "Mødestabilitet",
                            QuestionnaireTemplateFK = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6")
                        },
                        new
                        {
                            Id = 9,
                            AllowCustom = false,
                            Prompt = "Sygdom",
                            QuestionnaireTemplateFK = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6")
                        },
                        new
                        {
                            Id = 10,
                            AllowCustom = false,
                            Prompt = "Fravær",
                            QuestionnaireTemplateFK = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6")
                        },
                        new
                        {
                            Id = 11,
                            AllowCustom = false,
                            Prompt = "Praktikpladssøgning",
                            QuestionnaireTemplateFK = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6")
                        },
                        new
                        {
                            Id = 12,
                            AllowCustom = false,
                            Prompt = "Synlighed",
                            QuestionnaireTemplateFK = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6")
                        });
                });

            modelBuilder.Entity("Database.Models.QuestionnaireTemplateModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("LastUpated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id");

                    b.HasIndex("Title")
                        .IsUnique();

                    b.HasIndex("CreatedAt", "Id")
                        .IsDescending(true, false);

                    b.HasIndex("Title", "Id")
                        .IsDescending(true, false);

                    b.ToTable("QuestionnaireTemplate");

                    b.HasData(
                        new
                        {
                            Id = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                            CreatedAt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD",
                            LastUpated = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Title = "Evaluering af SKP-elever"
                        });
                });

            modelBuilder.Entity("Database.Models.TrackedRefreshTokenModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("bit");

                    b.Property<byte[]>("Token")
                        .IsRequired()
                        .HasColumnType("varbinary(900)");

                    b.Property<int?>("UserBaseModelId")
                        .HasColumnType("int");

                    b.Property<Guid>("UserGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ValidFrom")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Token");

                    b.HasIndex("UserBaseModelId");

                    b.ToTable("TrackedRefreshToken");
                });

            modelBuilder.Entity("Database.Models.UserBaseModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("nvarchar(13)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Permissions")
                        .HasColumnType("int");

                    b.Property<string>("PrimaryRole")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Guid")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("User");

                    b.HasDiscriminator().HasValue("UserBaseModel");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Database.Models.ActiveQuestionnaireStudentResponseModel", b =>
                {
                    b.HasBaseType("Database.Models.ActiveQuestionnaireResponseBaseModel");

                    b.HasIndex("ActiveQuestionnaireFK");

                    b.ToTable("ActiveQuestionnaireResponse");

                    b.HasDiscriminator().HasValue("ActiveQuestionnaireStudentResponseModel");
                });

            modelBuilder.Entity("Database.Models.ActiveQuestionnaireTeacherResponseModel", b =>
                {
                    b.HasBaseType("Database.Models.ActiveQuestionnaireResponseBaseModel");

                    b.HasIndex("ActiveQuestionnaireFK");

                    b.ToTable("ActiveQuestionnaireResponse");

                    b.HasDiscriminator().HasValue("ActiveQuestionnaireTeacherResponseModel");
                });

            modelBuilder.Entity("Database.Models.StudentModel", b =>
                {
                    b.HasBaseType("Database.Models.UserBaseModel");

                    b.ToTable("User");

                    b.HasDiscriminator().HasValue("StudentModel");
                });

            modelBuilder.Entity("Database.Models.TeacherModel", b =>
                {
                    b.HasBaseType("Database.Models.UserBaseModel");

                    b.ToTable("User");

                    b.HasDiscriminator().HasValue("TeacherModel");
                });

            modelBuilder.Entity("Database.Models.ActiveQuestionnaireModel", b =>
                {
                    b.HasOne("Database.Models.QuestionnaireTemplateModel", "QuestionnaireTemplate")
                        .WithMany("ActiveQuestionnaires")
                        .HasForeignKey("QuestionnaireTemplateFK")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Database.Models.StudentModel", "Student")
                        .WithMany("ActiveQuestionnaires")
                        .HasForeignKey("StudentFK")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Database.Models.TeacherModel", "Teacher")
                        .WithMany("ActiveQuestionnaires")
                        .HasForeignKey("TeacherFK")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("QuestionnaireTemplate");

                    b.Navigation("Student");

                    b.Navigation("Teacher");
                });

            modelBuilder.Entity("Database.Models.ActiveQuestionnaireResponseBaseModel", b =>
                {
                    b.HasOne("Database.Models.QuestionnaireOptionModel", "Option")
                        .WithMany()
                        .HasForeignKey("OptionFK");

                    b.HasOne("Database.Models.QuestionnaireQuestionModel", "Question")
                        .WithMany()
                        .HasForeignKey("QuestionFK")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Option");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("Database.Models.QuestionnaireOptionModel", b =>
                {
                    b.HasOne("Database.Models.QuestionnaireQuestionModel", "Question")
                        .WithMany("Options")
                        .HasForeignKey("QuestionFK")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("Database.Models.QuestionnaireQuestionModel", b =>
                {
                    b.HasOne("Database.Models.QuestionnaireTemplateModel", "QuestionnaireTemplate")
                        .WithMany("Questions")
                        .HasForeignKey("QuestionnaireTemplateFK")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("QuestionnaireTemplate");
                });

            modelBuilder.Entity("Database.Models.TrackedRefreshTokenModel", b =>
                {
                    b.HasOne("Database.Models.UserBaseModel", null)
                        .WithMany("TrackedRefreshTokens")
                        .HasForeignKey("UserBaseModelId");
                });

            modelBuilder.Entity("Database.Models.ActiveQuestionnaireStudentResponseModel", b =>
                {
                    b.HasOne("Database.Models.ActiveQuestionnaireModel", "ActiveQuestionnaire")
                        .WithMany("StudentAnswers")
                        .HasForeignKey("ActiveQuestionnaireFK")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ActiveQuestionnaire");
                });

            modelBuilder.Entity("Database.Models.ActiveQuestionnaireTeacherResponseModel", b =>
                {
                    b.HasOne("Database.Models.ActiveQuestionnaireModel", "ActiveQuestionnaire")
                        .WithMany("TeacherAnswers")
                        .HasForeignKey("ActiveQuestionnaireFK")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ActiveQuestionnaire");
                });

            modelBuilder.Entity("Database.Models.ActiveQuestionnaireModel", b =>
                {
                    b.Navigation("StudentAnswers");

                    b.Navigation("TeacherAnswers");
                });

            modelBuilder.Entity("Database.Models.QuestionnaireQuestionModel", b =>
                {
                    b.Navigation("Options");
                });

            modelBuilder.Entity("Database.Models.QuestionnaireTemplateModel", b =>
                {
                    b.Navigation("ActiveQuestionnaires");

                    b.Navigation("Questions");
                });

            modelBuilder.Entity("Database.Models.UserBaseModel", b =>
                {
                    b.Navigation("TrackedRefreshTokens");
                });

            modelBuilder.Entity("Database.Models.StudentModel", b =>
                {
                    b.Navigation("ActiveQuestionnaires");
                });

            modelBuilder.Entity("Database.Models.TeacherModel", b =>
                {
                    b.Navigation("ActiveQuestionnaires");
                });
#pragma warning restore 612, 618
        }
    }
}
