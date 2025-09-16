using Database.Models;
using Database.Utils;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Database.Seeders;

public class SingleStudentDataSeeder(ModelBuilder modelBuilder) : IDataSeeder<ActiveQuestionnaireStudentResponseModel>
{
    public void InitializeData()
    {
        List<ActiveQuestionnaireStudentResponseModel> defaultTemplate = IDataSeeder<ActiveQuestionnaireStudentResponseModel>.LoadSeed(CreateInstances());

        Console.WriteLine(defaultTemplate[0]);

        if (defaultTemplate is not null)
        {
            modelBuilder.Entity<ActiveQuestionnaireStudentResponseModel>().HasData(defaultTemplate);
        }
        else
        {
            Console.WriteLine("Der er ik noget");
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

    Guid activeQuestionnaireFK = Guid.Parse("3BEA93C1-7C90-4C26-E329-08DDDF06E1A3");

    private List<ActiveQuestionnaireStudentResponseModel> CreateInstances()
    {
        List<ActiveQuestionnaireStudentResponseModel> respondes = [];
        respondes.Add( new() { Id = ID1q1, QuestionFK = questionFK1, OptionFK = optionFK1Q1, ActiveQuestionnaireFK = activeQuestionnaireFK } );
        respondes.Add( new() { Id = ID1q2, QuestionFK = questionFK2, OptionFK = optionFK1Q2, ActiveQuestionnaireFK = activeQuestionnaireFK } );
        respondes.Add( new() { Id = ID1q3, QuestionFK = questionFK3, OptionFK = optionFK1Q3, ActiveQuestionnaireFK = activeQuestionnaireFK } );
 
        respondes.Add( new() { Id = ID2q1, QuestionFK = questionFK1, OptionFK = optionFK2Q1, ActiveQuestionnaireFK = activeQuestionnaireFK } );
        respondes.Add( new() { Id = ID2q2, QuestionFK = questionFK2, OptionFK = optionFK2Q2, ActiveQuestionnaireFK = activeQuestionnaireFK } );
        respondes.Add( new() { Id = ID2q3, QuestionFK = questionFK3, OptionFK = optionFK2Q3, ActiveQuestionnaireFK = activeQuestionnaireFK } );        
        
        respondes.Add( new() { Id = ID3q1, QuestionFK = questionFK1, OptionFK = optionFK3Q1, ActiveQuestionnaireFK = activeQuestionnaireFK } );
        respondes.Add( new() { Id = ID3q2, QuestionFK = questionFK2, OptionFK = optionFK3Q2, ActiveQuestionnaireFK = activeQuestionnaireFK } );
        respondes.Add( new() { Id = ID3q3, QuestionFK = questionFK3, OptionFK = optionFK3Q3, ActiveQuestionnaireFK = activeQuestionnaireFK } );

        Console.WriteLine(respondes[0].ActiveQuestionnaireFK);
        return respondes;
    }
    
}

