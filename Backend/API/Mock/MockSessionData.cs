namespace API.Mock;

public class MockSessionData
{
    public List<MockedUser> FilteredUsers { get; set; } = [];
    public int CurrentIndex { get; set; } = 0;
}
