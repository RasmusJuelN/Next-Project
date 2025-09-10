using System.Net;

namespace API.Exceptions.LDAP;

public class ConnectionErrorException(string? error) : BaseExcception(error ?? "Could not connect to the LDAP server.", HttpStatusCode.ServiceUnavailable)
{
}