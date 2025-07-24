// Helpers/ErrorHelper.cs
namespace PaySecure.Helpers;

public static class ErrorHelper
{
    public static string GetFriendlyErrorMessage(Exception exception)
    {
        return exception switch
        {
            HttpRequestException => Constants.Messages.NetworkError,
            UnauthorizedAccessException => "No tienes permisos para realizar esta acción",
            ArgumentException => "Los datos proporcionados no son válidos",
            InvalidOperationException => "No se puede realizar esta operación en este momento",
            TimeoutException => "La operación tardó demasiado tiempo. Inténtalo de nuevo",
            _ => "Ha ocurrido un error inesperado. Inténtalo de nuevo"
        };
    }

    public static async Task HandleErrorAsync(Exception exception, string context = "")
    {
        var friendlyMessage = GetFriendlyErrorMessage(exception);
        var title = string.IsNullOrEmpty(context) ? "Error" : $"Error en {context}";

        System.Diagnostics.Debug.WriteLine($"Error in {context}: {exception}");

        await Shell.Current.DisplayAlert(title, friendlyMessage, "OK");
    }

    public static void LogError(Exception exception, string context = "", Dictionary<string, object>? additionalData = null)
    {
        var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR in {context}: {exception.Message}";

        if (additionalData != null)
        {
            foreach (var kvp in additionalData)
            {
                logMessage += $"\n  {kvp.Key}: {kvp.Value}";
            }
        }

        logMessage += $"\nStackTrace: {exception.StackTrace}";

        System.Diagnostics.Debug.WriteLine(logMessage);

        // En producción, aquí enviarías los logs a un servicio como Application Insights, Sentry, etc.
    }
}