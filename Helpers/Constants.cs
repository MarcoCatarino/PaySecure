// Helpers/Constants.cs
using System.Text;
using System.Text.RegularExpressions;

namespace PaySecure.Helpers;

public static class Constants
{
    // Configuración de Stripe
    public static class Stripe
    {
        // IMPORTANTE: En producción, usar variables de entorno
        public const string PublishableKey = "pk_test_tu_clave_publica_aqui";
        public const string SecretKey = "sk_test_tu_clave_secreta_aqui";
        public const string WebhookSecret = "whsec_tu_webhook_secret_aqui";

        // Configuración para México
        public const string DefaultCurrency = "mxn";
        public const string CountryCode = "MX";
    }

    // Configuración de la base de datos
    public static class Database
    {
        public const string DatabaseFilename = "paySecure.db";
        public const SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }

    // Mensajes de la aplicación
    public static class Messages
    {
        public const string BiometricPrompt = "Confirma tu identidad para continuar";
        public const string PaymentPrompt = "Confirma el pago de {0:C} MXN";
        public const string DeleteConfirm = "¿Estás seguro que deseas eliminar este elemento?";
        public const string LogoutConfirm = "¿Estás seguro que deseas cerrar sesión?";

        // Errores comunes
        public const string NetworkError = "Error de conexión. Verifica tu internet.";
        public const string BiometricNotAvailable = "Autenticación biométrica no disponible";
        public const string PaymentFailed = "Error al procesar el pago";
        public const string ValidationError = "Por favor completa todos los campos requeridos";
    }

    // Configuración de la aplicación
    public static class App
    {
        public const string AppName = "PaySecure";
        public const string Version = "1.0.0";
        public const string DeveloperEmail = "developer@paySecure.com";
        public const string SupportUrl = "https://paySecure.com/support";
    }
}