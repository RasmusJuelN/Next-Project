using Database.DTO.User;

namespace Database.DTO.ActiveQuestionnaire;

public record class ResponseBase
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
}

public record class StudentResponse : ResponseBase
{
    public required Student Student { get; set; }
    public required List<StudentAnswer> Answers { get; set; }
}

public record class TeacherResponse : ResponseBase
{
    public required Student Student { get; set; }
    public required Teacher Teacher { get; set; }
    public required List<TeacherAnswer> Answers { get; set; }
}



public record class FullResponse : ResponseBase
{
    public required Student Student { get; set; }
    public required Teacher Teacher { get; set; }
    public required List<FullAnswer> Answers { get; set; }
}

public record class FullResponseDate : ResponseBase
{
    public DateTime? StudentCompletedAt { get; set; }
    public required List<FullAnswer> Answers { get; set; }
}

//#################################################//

public record class Student
{
    public required FullUser User { get; set; }
    public required DateTime CompletedAt { get; set; }
}

public record class Teacher
{
    public required FullUser User { get; set; }
    public required DateTime CompletedAt { get; set; }
}

public record AnswerBase
{
    public required string Question { get; set; }
}

public record StudentAnswer : AnswerBase
{
    public required string StudentResponse { get; set; }
    public required bool IsStudentResponseCustom { get; set; }
}

public record TeacherAnswer : StudentAnswer
{
    public required string TeacherResponse { get; set; }
    public required bool IsTeacherResponseCustom { get; set; }
}

public record FullAnswer : TeacherAnswer {};