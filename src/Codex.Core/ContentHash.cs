using System.Security.Cryptography;
using System.Text;

namespace Codex.Core;

/// <summary>
/// A content-addressed identity. The SHA-256 hash of an artifact's canonical byte representation.
/// Two artifacts with identical content have identical hashes. This is the foundation of
/// the Codex repository: names are derived from content, not assigned.
/// </summary>
public readonly record struct ContentHash : IComparable<ContentHash>, IEquatable<ContentHash>
{
    private readonly byte[] m_bytes;

    private ContentHash(byte[] bytes)
    {
        m_bytes = bytes;
    }

    /// <summary>The raw 32-byte SHA-256 hash.</summary>
    public ReadOnlySpan<byte> Bytes => m_bytes;

    /// <summary>Compute the content hash of arbitrary bytes.</summary>
    public static ContentHash Of(ReadOnlySpan<byte> data)
    {
        byte[] hash = SHA256.HashData(data);
        return new ContentHash(hash);
    }

    /// <summary>Compute the content hash of a UTF-8 string.</summary>
    public static ContentHash Of(string text)
    {
        return Of(Encoding.UTF8.GetBytes(text));
    }

    /// <summary>The hash as a lowercase hex string.</summary>
    public string ToHex()
    {
        return Convert.ToHexString(m_bytes).ToLowerInvariant();
    }

    /// <summary>Parse a hex string back into a ContentHash.</summary>
    public static ContentHash FromHex(string hex)
    {
        return new ContentHash(Convert.FromHexString(hex));
    }

    /// <summary>A short prefix for display (first 8 hex chars).</summary>
    public string ToShortHex() => ToHex()[..16];

    public int CompareTo(ContentHash other)
    {
        for (int i = 0; i < m_bytes.Length && i < other.m_bytes.Length; i++)
        {
            int cmp = m_bytes[i].CompareTo(other.m_bytes[i]);
            if (cmp != 0)
            {
                return cmp;
            }
        }
        return m_bytes.Length.CompareTo(other.m_bytes.Length);
    }

    public bool Equals(ContentHash other)
    {
        return m_bytes.AsSpan().SequenceEqual(other.m_bytes);
    }

    public override int GetHashCode()
    {
        if (m_bytes is null || m_bytes.Length < 4)
        {
            return 0;
        }
        return BitConverter.ToInt32(m_bytes, 0);
    }

    public override string ToString() => ToShortHex();
}
