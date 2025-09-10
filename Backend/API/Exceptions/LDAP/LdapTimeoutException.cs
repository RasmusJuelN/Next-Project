using System;

namespace API.Exceptions.LDAP;

public class LdapTimeoutException(string? error) : BaseExcception(error ?? "The LDAP server did not respond in a timely manner.", System.Net.HttpStatusCode.RequestTimeout)
{

}
