using System.Security.Cryptography;
using System.Text;

namespace API.Utils;

public class Crypto(string value, Encoding? encoding = null)
{
    private readonly string _value = value;
    private readonly Encoding _encoding = encoding ?? Encoding.UTF8;

    public byte[] ToSha256()
    {
        return SHA256.HashData(GetBytes());
    }

    public static byte[] ToSha256(string value, Encoding encoding)
    {
        return SHA256.HashData(GetBytes(value, encoding));
    }

    public static byte[] ToSha256(string value)
    {
        return ToSha256(value, Encoding.UTF8);
    }

    private byte[] GetBytes()
    {
        return _encoding.GetBytes(_value);
    }

    private static byte[] GetBytes(string value, Encoding encoding)
    {
        return encoding.GetBytes(value);
    }
}
