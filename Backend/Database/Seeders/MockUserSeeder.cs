namespace Database.Seeders;

[SeederOrder(1)]
public class MockStudentSeeder(ModelBuilder modelBuilder) : IDataSeeder<StudentModel>
{
    private int _userId =  -1;
    public void InitializeData()
    {
        List<StudentModel> students = CreateStudents();
        List<TeacherModel> teachers = CreateTeachers();

        modelBuilder.Entity<StudentModel>().HasData(students);
        modelBuilder.Entity<TeacherModel>().HasData(teachers);
    }

    private List<StudentModel> CreateStudents()
    {
        List<MockedUser> mockedUsers = IDataSeeder<List<MockedUser>>.LoadSeed("mocked_user_data.json") ?? [];

        return [.. mockedUsers.Where(mu => mu.Role == UserRoles.Student).
            Take(20).
            Select(mu => new StudentModel()
            {
                Id = _userId--,
                Guid = mu.Id,
                UserName = mu.Username,
                FullName = mu.FullName,
                PrimaryRole = mu.Role,
                Permissions = UserPermissions.Student
            })];
    }

    private List<TeacherModel> CreateTeachers()
    {
        List<MockedUser> mockedUsers = IDataSeeder<List<MockedUser>>.LoadSeed("mocked_user_data.json") ?? [];

        return [.. mockedUsers.Where(mu => mu.Role == UserRoles.Teacher).
            Take(20).
            Select(mu => new TeacherModel()
            {
                Id = _userId--,
                Guid = mu.Id,
                UserName = mu.Username,
                FullName = mu.FullName,
                PrimaryRole = mu.Role,
                Permissions = UserPermissions.Teacher
            })];
    }
}
