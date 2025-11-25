
namespace API.Utils;

/// <summary>
/// Provides cryptographic hashing utilities for string values with configurable encoding.
/// </summary>
/// <param name="value">The string value to be hashed.</param>
/// <param name="encoding">The encoding to use when converting the string to bytes. Defaults to UTF-8 if not specified.</param>
/// <remarks>
/// This class encapsulates a string value and its encoding, providing methods to generate SHA-256 hashes.
/// It supports both instance methods for the encapsulated value and static methods for one-off hashing operations.
/// </remarks>
public class Crypto(string value, Encoding? encoding = null)
{
    private readonly string _value = value;
    private readonly Encoding _encoding = encoding ?? Encoding.UTF8;

    /// <summary>
    /// Computes the SHA-256 hash of the current object's byte representation.
    /// </summary>
    /// <returns>A byte array containing the SHA-256 hash of the object.</returns>
    public byte[] ToSha256()
    {
        return SHA256.HashData(GetBytes());
    }

    /// <summary>
    /// Computes the SHA-256 hash of the specified string value using the provided encoding.
    /// </summary>
    /// <param name="value">The string value to hash.</param>
    /// <param name="encoding">The encoding to use when converting the string to bytes.</param>
    /// <returns>A byte array containing the SHA-256 hash of the input string.</returns>
    public static byte[] ToSha256(string value, Encoding encoding)
    {
        return SHA256.HashData(GetBytes(value, encoding));
    }

    /// <summary>
    /// Computes the SHA-256 hash of the specified string using UTF-8 encoding.
    /// </summary>
    /// <param name="value">The string to compute the hash for.</param>
    /// <returns>A byte array containing the SHA-256 hash of the input string.</returns>
    public static byte[] ToSha256(string value)
    {
        return ToSha256(value, Encoding.UTF8);
    }

    /// <summary>
    /// Converts the internal string value to a byte array using the configured encoding.
    /// </summary>
    /// <returns>A byte array representation of the internal string value.</returns>
    private byte[] GetBytes()
    {
        return _encoding.GetBytes(_value);
    }

    /// <summary>
    /// Converts a string value to a byte array using the specified encoding.
    /// </summary>
    /// <param name="value">The string to convert to bytes.</param>
    /// <param name="encoding">The encoding to use for the conversion.</param>
    /// <returns>A byte array representation of the input string using the specified encoding.</returns>
    private static byte[] GetBytes(string value, Encoding encoding)
    {
        return encoding.GetBytes(value);
    }
}
