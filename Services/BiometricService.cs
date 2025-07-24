using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace PaySecure.Services;

public interface IBiometricService
{
    Task<bool> IsAvailableAsync();
    Task<bool> AuthenticateAsync(string reason);
    Task<BiometricCapabilities> GetCapabilitiesAsync();
}

public class BiometricService : IBiometricService
{
    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            return await CrossFingerprint.Current.IsAvailableAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking biometric availability: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> AuthenticateAsync(string reason)
    {
        try
        {
            var request = new AuthenticationRequestConfiguration(
                "Autenticación Biométrica",
                reason)
            {
                AllowAlternativeAuthentication = true,
                CancelTitle = "Cancelar",
                FallbackTitle = "Usar PIN"
            };

            var result = await CrossFingerprint.Current.AuthenticateAsync(request);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Authentication error: {ex.Message}");
            return false;
        }
    }

    public async Task<BiometricCapabilities> GetCapabilitiesAsync()
    {
        try
        {
            if (await IsAvailableAsync())
            {
                //return await CrossFingerprint.Current.GetAvailabilityAsync();
                return (BiometricCapabilities)await CrossFingerprint.Current.GetAvailabilityAsync();
            }
            return BiometricCapabilities.Unknown;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting capabilities: {ex.Message}");
            return BiometricCapabilities.Unknown;
        }
    }
}

// Enumeración personalizada para capacidades
public enum BiometricCapabilities
{
    Unknown,
    Available,
    NotEnrolled,
    NotAvailable,
    SecurityNotEnabled
}
}