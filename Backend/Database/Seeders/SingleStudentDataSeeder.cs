using Database.Models;
using Database.Utils;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Database.Seeders;

public class SingleStudentDataSeeder(ModelBuilder modelBuilder) : IDataSeeder<ActiveQuestionnaireStudentResponseModel>
{
    public void InitializeData()
    {
        List<ActiveQuestionnaireModel> defaultTemplate1 = IDataSeeder<ActiveQuestionnaireModel>.LoadSeed(CreateActiveQuestionnaires());
        List<ActiveQuestionnaireStudentResponseModel> defaultTemplate2 = IDataSeeder<ActiveQuestionnaireStudentResponseModel>.LoadSeed(CreateInstances());

        if (defaultTemplate1 is not null)
        {
            modelBuilder.Entity<ActiveQuestionnaireModel>().HasData(defaultTemplate1);
            if (defaultTemplate2 is not null)
            {
                modelBuilder.Entity<ActiveQuestionnaireStudentResponseModel>().HasData(defaultTemplate2);
            }
        }

    }

    int ID1q1 = -1;
    int ID1q2 = -2;
    int ID1q3 = -3;

    int ID2q1 = -4;
    int ID2q2 = -5;
    int ID2q3 = -6;

    int ID3q1 = -7;
    int ID3q2 = -8;
    int ID3q3 = -9;

    int questionFK1 = 13;
    int questionFK2 = 14;
    int questionFK3 = 15;

    int optionFK1Q1 = 45;
    int optionFK2Q1 = 43;
    int optionFK3Q1 = 44;

    int optionFK1Q2 = 47;
    int optionFK2Q2 = 49;
    int optionFK3Q2 = 48;

    int optionFK1Q3 = 52;
    int optionFK2Q3 = 55;
    int optionFK3Q3 = 56;



    private List<ActiveQuestionnaireStudentResponseModel> CreateInstances()
    {
        List<ActiveQuestionnaireStudentResponseModel> respondes = [];
        respondes.Add(new() { Id = ID1q1, QuestionFK = questionFK3, OptionFK = optionFK1Q3, ActiveQuestionnaireFK = activeQuestID3 });
        respondes.Add(new() { Id = ID1q2, QuestionFK = questionFK2, OptionFK = optionFK1Q2, ActiveQuestionnaireFK = activeQuestID2 });
        respondes.Add(new() { Id = ID1q3, QuestionFK = questionFK1, OptionFK = optionFK1Q1, ActiveQuestionnaireFK = activeQuestID1 });

        respondes.Add(new() { Id = ID2q1, QuestionFK = questionFK3, OptionFK = optionFK2Q3, ActiveQuestionnaireFK = activeQuestID3 });
        respondes.Add(new() { Id = ID2q2, QuestionFK = questionFK2, OptionFK = optionFK2Q2, ActiveQuestionnaireFK = activeQuestID2 });
        respondes.Add(new() { Id = ID2q3, QuestionFK = questionFK1, OptionFK = optionFK2Q1, ActiveQuestionnaireFK = activeQuestID1 });

        respondes.Add(new() { Id = ID3q1, QuestionFK = questionFK3, OptionFK = optionFK3Q3, ActiveQuestionnaireFK = activeQuestID3 });
        respondes.Add(new() { Id = ID3q2, QuestionFK = questionFK2, OptionFK = optionFK3Q2, ActiveQuestionnaireFK = activeQuestID2 });
        respondes.Add(new() { Id = ID3q3, QuestionFK = questionFK1, OptionFK = optionFK3Q1, ActiveQuestionnaireFK = activeQuestID1 });

        Console.WriteLine(respondes[0].ActiveQuestionnaireFK);
        return respondes;
    }

    Guid activeQuestID1 = Guid.Parse("6135b27d-37c1-420c-b3ef-39f76649d515");
    Guid activeQuestID2 = Guid.Parse("77a08073-bd8e-45c0-90ea-88dc3f494bf8");
    Guid activeQuestID3 = Guid.Parse("fee3e2d9-0d4c-4509-bf50-f3251c8d98ea");

    string title1 = "Bedste Land";

    int studentFK = 1;

    int teacherFK = 2;

    Guid questionTemplateFK = Guid.Parse("569E97BA-40CE-4D27-00F5-08DDD8C9910C");

    string description = "Description for the new template";

    DateTime todayMake1 = DateTime.Parse("2025 - 08 - 20 09:58:30.5360158");
    DateTime todayMake2 = DateTime.Parse("2025 - 08 - 21 09:58:30.5360158");
    DateTime todayMake3 = DateTime.Parse("2024 - 08 - 22 09:58:30.5360158");

    Guid groupID1 = Guid.Parse("E66E1023-BE16-4CF6-ADA2-B5D1F5DBBF59");

    private List<ActiveQuestionnaireModel> CreateActiveQuestionnaires()
    {
        List<ActiveQuestionnaireModel> activeQuestions = [];
        activeQuestions.Add(new() { Id = activeQuestID1, Title = title1, StudentFK = studentFK, TeacherFK = teacherFK, 
                                    QuestionnaireTemplateFK = questionTemplateFK, StudentCompletedAt = todayMake1, TeacherCompletedAt = todayMake1,Description = description, GroupId = groupID1 
                                    });

        activeQuestions.Add(new() { Id = activeQuestID2, Title = title1, StudentFK = studentFK, TeacherFK = teacherFK, 
                                    QuestionnaireTemplateFK = questionTemplateFK, StudentCompletedAt = todayMake2, TeacherCompletedAt = todayMake2, Description = description, GroupId = groupID1 
                                    });

        activeQuestions.Add(new() { Id = activeQuestID3, Title = title1, StudentFK = studentFK, TeacherFK = teacherFK, 
                                    QuestionnaireTemplateFK = questionTemplateFK, StudentCompletedAt = todayMake3, TeacherCompletedAt = todayMake3, Description = description, GroupId = groupID1 
                                    });


        return activeQuestions;
    }


}

