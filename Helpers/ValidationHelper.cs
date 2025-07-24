// Helpers/ValidationHelper.cs
using System.Text.RegularExpressions;

namespace PaySecure.Helpers;

public static class ValidationHelper
{
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return emailRegex.IsMatch(email);
    }

    public static bool IsValidAmount(decimal amount)
    {
        return amount > 0 && amount <= 999999.99m;
    }

    public static bool IsValidPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        // Formato mexicano: +52 55 1234 5678 o variaciones
        var phoneRegex = new Regex(@"^(\+52\s?)?(\d{2}\s?\d{4}\s?\d{4}|\d{10})$");
        return phoneRegex.IsMatch(phone.Replace(" ", "").Replace("-", ""));
    }

    public static bool IsValidDescription(string description)
    {
        return !string.IsNullOrWhiteSpace(description) && description.Length >= 3 && description.Length <= 200;
    }

    public static string FormatMexicanCurrency(decimal amount)
    {
        return amount.ToString("C", new System.Globalization.CultureInfo("es-MX"));
    }

    public static string FormatMexicanPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return string.Empty;

        var cleanPhone = phone.Replace(" ", "").Replace("-", "").Replace("+52", "");

        if (cleanPhone.Length == 10)
        {
            return $"+52 {cleanPhone.Substring(0, 2)} {cleanPhone.Substring(2, 4)} {cleanPhone.Substring(6, 4)}";
        }

        return phone;
    }
}