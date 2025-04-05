using System.Security.Cryptography;
using System.Text;

namespace SpotiExport;

public class PkceChallengeGenerator
{
    public string ChallengeMethod => "S256";
    public string GenerateCodeVerifier()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[64];
        rng.GetBytes(bytes);
        return Base64UrlEncode(bytes);
    }

    public string GenerateCodeChallenge(string verifier)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(verifier));
        return Base64UrlEncode(bytes);
    }

    static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}