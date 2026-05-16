using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace Zyntra.Shared.Helpers;
public static class PasswordHasher
{
    private const int SaltSize = 32;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public static (string Hash, string Salt, int Iterations) HashPassword(string password)
    {
        byte[] saltBytes = new byte[SaltSize];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256);
        byte[] hashBytes = pbkdf2.GetBytes(KeySize);
        return (Hash: Convert.ToBase64String(hashBytes), Salt: Convert.ToBase64String(saltBytes), Iterations: Iterations);
    }

    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        byte[] saltBytes = Convert.FromBase64String(storedSalt);
        byte[] storedHashBytes = Convert.FromBase64String(storedHash);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256);
        byte[] computedHash = pbkdf2.GetBytes(KeySize);
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
    }

    public static bool Validate(string value, string salt, string hash)
    {
        return CreateHash(value, salt) == hash;
    }

    public static string CreateSalt()
    {
        byte[] randomBytes = new byte[128 / 8];
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }

    public static string CreateHash(string value, string salt)
    {
        var valueBytes = KeyDerivation.Pbkdf2(password: value, salt: Encoding.UTF8.GetBytes(salt), prf: KeyDerivationPrf.HMACSHA512, iterationCount: 10000, numBytesRequested: 256 / 8);

        return Convert.ToBase64String(valueBytes);
    }
}