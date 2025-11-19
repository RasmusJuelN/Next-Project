using Database.Enums;
using Database.Models;
using Database.Utils;
using Microsoft.EntityFrameworkCore;

namespace Database.Seeders;

public class StudentQuestionnaireResponseSeeder(ModelBuilder modelBuilder) : IDataSeeder<ActiveQuestionnaireModel>
{
    public void InitializeData()
    {
        List<ActiveQuestionnaireModel> seededActiveQuestionnaires = IDataSeeder<ActiveQuestionnaireModel>.LoadSeed(CreateModels());
        if (seededActiveQuestionnaires is not null && seededActiveQuestionnaires.Count > 0)
        {
            bool studentSeeded = false;
            bool teacherSeeded = false;
            bool groupSeeded = false;
            bool templateSeeded = false;

            foreach (ActiveQuestionnaireModel activeQuestionnaire in seededActiveQuestionnaires)
            {
                if (!studentSeeded)
                {
                    modelBuilder.Entity<StudentModel>().HasData(activeQuestionnaire.Student!);
                    studentSeeded = true;
                }
                if (!teacherSeeded)
                {
                    modelBuilder.Entity<TeacherModel>().HasData(activeQuestionnaire.Teacher!);
                    teacherSeeded = true;
                }
                if (!groupSeeded)
                {
                    modelBuilder.Entity<QuestionnaireGroupModel>().HasData(activeQuestionnaire.Group!);
                    groupSeeded = true;
                }

                if (!templateSeeded)
                {
                    foreach (QuestionnaireQuestionModel templateQuestion in activeQuestionnaire.QuestionnaireTemplate!.Questions)
                    {
                        modelBuilder.Entity<QuestionnaireOptionModel>().HasData(templateQuestion.Options);
                        templateQuestion.Options = [];
                    }

                    modelBuilder.Entity<QuestionnaireQuestionModel>().HasData(activeQuestionnaire.QuestionnaireTemplate.Questions);
                    activeQuestionnaire.QuestionnaireTemplate.Questions = [];

                    modelBuilder.Entity<QuestionnaireTemplateModel>().HasData(activeQuestionnaire.QuestionnaireTemplate);
                    activeQuestionnaire.QuestionnaireTemplate = null;

                    templateSeeded = true;
                }


                modelBuilder.Entity<ActiveQuestionnaireStudentResponseModel>().HasData(activeQuestionnaire.StudentAnswers);
                modelBuilder.Entity<ActiveQuestionnaireTeacherResponseModel>().HasData(activeQuestionnaire.TeacherAnswers);

                activeQuestionnaire.Student = null;
                activeQuestionnaire.Teacher = null;
                activeQuestionnaire.Group = null;
                activeQuestionnaire.QuestionnaireTemplate = null;
                activeQuestionnaire.StudentAnswers = [];
                activeQuestionnaire.TeacherAnswers = [];

                modelBuilder.Entity<ActiveQuestionnaireModel>().HasData(activeQuestionnaire);
            }
        }
    }

    private static List<ActiveQuestionnaireModel> CreateModels()
    {
        // Questionnaire Template
        Guid templateId = Guid.Parse("69088ed6-4fa5-4e85-8d80-18334b7bfabf");
        string templateTitle = "Bedste Land";
        string templateDescription = "Description for the new template.";
        DateTime templateCreatedAt = DateTime.Parse("2025-08-19 09:58:30.5360158");
        DateTime templateLastUpdated = DateTime.Parse("2025-08-19 09:58:30.5360158");

        // Questionnaire Template Questions
        Dictionary<int, string> questions = new()
        {
            {
                -1,
                "Asien"
            },
            {
                -2,
                "Skandinavien"
            },
            {
                -3,
                "Østeuropa"
            },

        };

        // Questionnaire Template Options
        Dictionary<int, Dictionary<int, string>> options = new()
        {
            {
                questions.Keys.ElementAt(0),
                new Dictionary<int, string>{
                    {-1, "Japan"},
                    {-2, "Indien"},
                    {-3, "SydKorea"}
                }
            },
            {
                questions.Keys.ElementAt(1),
                new Dictionary<int, string>{
                    {-4, "Danmark"},
                    {-5, "Norge"},
                    {-6, "Sverige"}
                }
            },
            {
                questions.Keys.ElementAt(2),
                new Dictionary<int, string>{
                    {-7, "Rusland"},
                    {-8, "Polen"},
                    {-9, "Bulgarien"}
                }
            },
        };

        // Active Questionnaires
        List<Guid> activeQuestionnaireIds = [
            Guid.Parse("b65c922e-9ce0-4a7a-9e4f-9d98f0f4e213"),
            Guid.Parse("7ff68cc7-4702-4b54-9cf7-72c24fdb57b9"),
            Guid.Parse("76e83a1c-73e4-4572-b0e3-3023f503151f"),
            Guid.Parse("560fa037-52d2-4c8d-86f3-7467ed48f54d"),
            Guid.Parse("b812acd2-f43a-42f0-9a10-02158380c88c"),
            Guid.Parse("d9cf4a1e-83ef-4f3b-bf5b-ad25338dc094"),
            Guid.Parse("7c81756d-2ae8-41e8-ac79-824bc632c8a1"),
            Guid.Parse("4814723f-50af-4414-9c17-c79d7aac3831"),
            Guid.Parse("08062aef-1e18-4c86-ac07-46c9d579e750")
        ];
        List<DateTime> activatedAtTimes = [
            DateTime.Parse("2023 - 03 - 20 09:58:30.5360158"),
            DateTime.Parse("2023 - 06 - 20 09:58:30.5360158"),
            DateTime.Parse("2023 - 09 - 20 09:58:30.5360158"),
            DateTime.Parse("2024 - 03 - 20 09:58:30.5360158"),
            DateTime.Parse("2024 - 06 - 20 09:58:30.5360158"),
            DateTime.Parse("2024 - 09 - 20 09:58:30.5360158"),
            DateTime.Parse("2025 - 03 - 20 09:58:30.5360158"),
            DateTime.Parse("2025 - 06 - 20 09:58:30.5360158"),
            DateTime.Parse("2025 - 09 - 20 09:58:30.5360158"),
        ];
        List<DateTime> completedAtTime = [
            DateTime.Parse("2023 - 03 - 21 09:58:30.5360158"),
            DateTime.Parse("2023 - 06 - 21 09:58:30.5360158"),
            DateTime.Parse("2023 - 09 - 21 09:58:30.5360158"),
            DateTime.Parse("2024 - 03 - 21 09:58:30.5360158"),
            DateTime.Parse("2024 - 06 - 21 09:58:30.5360158"),
            DateTime.Parse("2024 - 09 - 21 09:58:30.5360158"),
            DateTime.Parse("2025 - 03 - 21 09:58:30.5360158"),
            DateTime.Parse("2025 - 06 - 21 09:58:30.5360158"),
            DateTime.Parse("2025 - 09 - 21 09:58:30.5360158")
        ];

        // Questionnaire Group
        Guid groupId = Guid.Parse("310c585d-0c9a-4679-802e-1c1538475636");
        string groupName = "Default Group";
        DateTime groupCreatedAt = DateTime.Parse("2025-08-19 09:58:30.5360158");

        // Active Questionnaire Student Responses. We need to set the ID of the response, the question foreign key, and option foreign key
        // Generate 9 responses: 3 questionnaires × 3 questions each = 9 total responses
        int responseIdCounter = -10;

        Random random = new(205732675);

        // Fixed "random" response pattern to simulate real-world scenarios
        // Each inner array represents responses for one questionnaire [Q1_option, Q2_option, Q3_option]
        // Options are 0-indexed within each question's available options
        int[][] studentResponsePattern = [
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)]
        ];

        int[][] teacherResponsePattern = [
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)],
            [random.Next(0, 3), random.Next(0, 3), random.Next(0, 3)]
        ];

        // Users
        int studentId = -1;
        int teacherId = -2;
        Guid studentGuid = Guid.Parse("8ccdeb24-9027-40f0-986f-a5d2d171469a");
        Guid teacherGuid = Guid.Parse("76889b1b-c762-482b-a1cb-2e4189dbb484");
        string studentUserName = "student1";
        string teacherUserName = "teacher1";
        string studentFullName = "Student One";
        string teacherFullName = "Teacher One";
        UserRoles studentRole = UserRoles.Student;
        UserRoles teacherRole = UserRoles.Teacher;
        UserPermissions studentPermissions = UserPermissions.Student;
        UserPermissions teacherPermissions = UserPermissions.Teacher;

        List<ActiveQuestionnaireModel> activeQuestionnaires = [];
        for (int i = 0; i < activeQuestionnaireIds.Count; i++)
        {
            activeQuestionnaires.Add(
                new ActiveQuestionnaireModel()
                {
                    Id = activeQuestionnaireIds[i],
                    Title = templateTitle,
                    Description = templateDescription,
                    StudentFK = studentId,
                    TeacherFK = teacherId,
                    QuestionnaireTemplateFK = templateId,
                    GroupId = groupId,
                    ActivatedAt = activatedAtTimes[i],
                    StudentCompletedAt = completedAtTime[i],
                    TeacherCompletedAt = completedAtTime[i],
                    QuestionnaireType = ActiveQuestionnaireType.Anonymous,
                    Student = new StudentModel()
                    {
                        Id = studentId,
                        Guid = studentGuid,
                        UserName = studentUserName,
                        FullName = studentFullName,
                        PrimaryRole = studentRole,
                        Permissions = studentPermissions
                    },
                    Teacher = new TeacherModel()
                    {
                        Id = teacherId,
                        Guid = teacherGuid,
                        UserName = teacherUserName,
                        FullName = teacherFullName,
                        PrimaryRole = teacherRole,
                        Permissions = teacherPermissions
                    },
                    QuestionnaireTemplate = new QuestionnaireTemplateModel()
                    {
                        Id = templateId,
                        Title = templateTitle,
                        Description = templateDescription,
                        CreatedAt = templateCreatedAt,
                        LastUpated = templateLastUpdated,
                        TemplateStatus = TemplateStatus.Finalized,
                        Questions = [.. questions.Select((q, questionIndex) => new QuestionnaireQuestionModel()
                        {
                            Id = q.Key,
                            Prompt = q.Value,
                            AllowCustom = false,
                            SortOrder = questionIndex,
                            QuestionnaireTemplateFK = templateId,
                            Options = [.. options[q.Key].Select((option, optionIndex) => new QuestionnaireOptionModel(){
                                Id = option.Key,
                                OptionValue = option.Key,
                                DisplayText = option.Value,
                                SortOrder = optionIndex,
                                QuestionFK = q.Key
                            })]
                        })]
                    },
                    Group = new QuestionnaireGroupModel()
                    {
                        GroupId = groupId,
                        TemplateId = templateId,
                        Name = groupName,
                        CreatedAt = groupCreatedAt
                    },
                    StudentAnswers = [.. questions.Keys.Select((questionFK, responseIndex) =>
                        new ActiveQuestionnaireStudentResponseModel()
                        {
                            Id = responseIdCounter--,
                            QuestionFK = questionFK,
                            OptionFK = options[questionFK].Keys.ElementAt(studentResponsePattern[i][responseIndex]),
                            ActiveQuestionnaireFK = activeQuestionnaireIds[i],
                            CustomResponse = null
                        })],
                    TeacherAnswers = [.. questions.Keys.Select((questionFK, responseIndex) =>
                        new ActiveQuestionnaireTeacherResponseModel()
                        {
                            Id = responseIdCounter--,
                            QuestionFK = questionFK,
                            OptionFK = options[questionFK].Keys.ElementAt(teacherResponsePattern[i][responseIndex]),
                            ActiveQuestionnaireFK = activeQuestionnaireIds[i],
                            CustomResponse = null
                        })]
                }
            );
        }

        return activeQuestionnaires;
    }
}
