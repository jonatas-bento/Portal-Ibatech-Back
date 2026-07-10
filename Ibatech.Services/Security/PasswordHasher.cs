// Ibatech.Services/Security/PasswordHasher.cs
using System.Security.Cryptography;
using System.Text;

namespace Ibatech.Services.Security;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public static string Hash(string senha)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(senha),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string senha, string senhaHash)
    {
        var partes = senhaHash.Split(':');
        if (partes.Length != 2) return false;

        var salt = Convert.FromBase64String(partes[0]);
        var hashEsperado = Convert.FromBase64String(partes[1]);
        var hashFornecido = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(senha),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return CryptographicOperations.FixedTimeEquals(hashFornecido, hashEsperado);
    }
}