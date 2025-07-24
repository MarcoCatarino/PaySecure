// Helpers/SecurityHelper.cs
using System.Security.Cryptography;
using System.Text;

namespace PaySecure.Helpers;

public static class SecurityHelper
{
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput.Equals(hashedPassword);
    }

    public static string GenerateTransactionId()
    {
        return $"TXN_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }

    public static string MaskCreditCard(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 4)
            return "****";

        return $"**** **** **** {cardNumber[^4..]}";
    }

    public static string MaskEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return "***@***.***";

        var parts = email.Split('@');
        var username = parts[0];
        var domain = parts[1];

        var maskedUsername = username.Length > 2
            ? $"{username[0]}***{username[^1]}"
            : "***";

        return $"{maskedUsername}@{domain}";
    }
}