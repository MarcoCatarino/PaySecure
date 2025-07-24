// ViewModels/LoginViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaySecure.Services;

namespace PaySecure.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IBiometricService _biometricService;

    [ObservableProperty]
    private string email = "demo@paySecure.com";

    [ObservableProperty]
    private bool isBiometricAvailable;

    [ObservableProperty]
    private string loginStatus = string.Empty;

    public LoginViewModel(IBiometricService biometricService)
    {
        _biometricService = biometricService;
        Title = "Iniciar Sesión";
        CheckBiometricAvailability();
    }

    private async void CheckBiometricAvailability()
    {
        IsBiometricAvailable = await _biometricService.IsAvailableAsync();
    }

    [RelayCommand]
    private async Task LoginWithBiometric()
    {
        if (!IsBiometricAvailable)
        {
            LoginStatus = "Autenticación biométrica no disponible";
            return;
        }

        IsBusy = true;
        LoginStatus = "Autenticando...";

        try
        {
            var success = await _biometricService.AuthenticateAsync(
                "Confirma tu identidad para acceder a PaySecure");

            if (success)
            {
                LoginStatus = "✅ Autenticación exitosa";
                await Shell.Current.GoToAsync("//main");
            }
            else
            {
                LoginStatus = "❌ Autenticación fallida";
            }
        }
        catch (Exception ex)
        {
            LoginStatus = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoginWithEmail()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            LoginStatus = "Por favor ingresa tu email";
            return;
        }

        IsBusy = true;
        LoginStatus = "Iniciando sesión...";

        try
        {
            // Simular autenticación por email
            await Task.Delay(1500);
            LoginStatus = "✅ Sesión iniciada";
            await Shell.Current.GoToAsync("//main");
        }
        catch (Exception ex)
        {
            LoginStatus = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}