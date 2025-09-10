using System.Net;

namespace API.Exceptions.LDAP;

public class InvalidCredentialsException(string? error) : BaseExcception(error ?? "Invalid credentials provided for LDAP authentication.", HttpStatusCode.Unauthorized)
{
}
