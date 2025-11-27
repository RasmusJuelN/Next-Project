
namespace Logging.LogEvents;

public class UserLogEvents : LogEventsBase
{
    public static readonly EventId UserLogIn = new(100, "UserLogIn");
    public static readonly EventId UserLogOut = new(101, "UserLogOut");
    public static readonly EventId UserLogInFailed = new(102, "UserLogInFailed");
    public static readonly EventId UserLogOutFailed = new(103, "UserLogOutFailed");
    public static readonly EventId UserTokenRefresh = new(104, "UserTokenRefresh");
    public static readonly EventId UserTokenRefreshFailed = new(105, "UserTokenRefreshFailed");
}
