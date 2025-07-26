// ViewModels/LoginViewModel.cs - Con registro y login real
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaySecure.Services;
using PaySecure.Models;
using PaySecure.Helpers;

namespace PaySecure.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IBiometricService _biometricService;
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private string email = "demo@paySecure.com";

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isBiometricAvailable;

    [ObservableProperty]
    private string loginStatus = string.Empty;

    [ObservableProperty]
    private bool isRegistering = false;

    // Campos para registro
    [ObservableProperty]
    private string registerName = string.Empty;

    [ObservableProperty]
    private string registerEmail = string.Empty;

    [ObservableProperty]
    private string registerPassword = string.Empty;

    [ObservableProperty]
    private string registerPhone = string.Empty;

    public LoginViewModel(IBiometricService biometricService, DatabaseService databaseService)
    {
        _biometricService = biometricService;
        _databaseService = databaseService;
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
                // Usar el último usuario logueado o demo
                var user = await _databaseService.GetUserByEmailAsync(Email);
                if (user != null)
                {
                    await SetCurrentUserAndNavigate(user);
                }
                else
                {
                    LoginStatus = "❌ Usuario no encontrado";
                }
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
        LoginStatus = "Verificando usuario...";

        try
        {
            var user = await _databaseService.GetUserByEmailAsync(Email);

            if (user == null)
            {
                LoginStatus = "❌ Usuario no encontrado. ¿Deseas registrarte?";
                return;
            }

            // Para simplicidad, permitir login solo con email válido
            // En producción, aquí verificarías password hash
            if (ValidationHelper.IsValidEmail(Email))
            {
                await SetCurrentUserAndNavigate(user);
            }
            else
            {
                LoginStatus = "❌ Email inválido";
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
    private void ShowRegister()
    {
        IsRegistering = true;
        LoginStatus = string.Empty;
        // Limpiar campos
        RegisterName = string.Empty;
        RegisterEmail = string.Empty;
        RegisterPassword = string.Empty;
        RegisterPhone = string.Empty;
    }

    [RelayCommand]
    private void ShowLogin()
    {
        IsRegistering = false;
        LoginStatus = string.Empty;
    }

    [RelayCommand]
    private async Task RegisterUser()
    {
        // Validaciones
        if (string.IsNullOrWhiteSpace(RegisterName))
        {
            LoginStatus = "❌ El nombre es requerido";
            return;
        }

        if (!ValidationHelper.IsValidEmail(RegisterEmail))
        {
            LoginStatus = "❌ Email inválido";
            return;
        }

        if (string.IsNullOrWhiteSpace(RegisterPassword) || RegisterPassword.Length < 4)
        {
            LoginStatus = "❌ La contraseña debe tener al menos 4 caracteres";
            return;
        }

        IsBusy = true;
        LoginStatus = "Registrando usuario...";

        try
        {
            // Verificar si el usuario ya existe
            var existingUser = await _databaseService.GetUserByEmailAsync(RegisterEmail);
            if (existingUser != null)
            {
                LoginStatus = "❌ Ya existe un usuario con este email";
                return;
            }

            // Crear nuevo usuario
            var newUser = new User
            {
                Name = RegisterName,
                Email = RegisterEmail,
                Phone = ValidationHelper.IsValidPhone(RegisterPhone) ?
                    ValidationHelper.FormatMexicanPhone(RegisterPhone) : RegisterPhone,
                CreatedAt = DateTime.Now,
                IsBiometricEnabled = IsBiometricAvailable
            };

            await _databaseService.CreateUserAsync(newUser);

            LoginStatus = "✅ Usuario registrado exitosamente";

            // Auto-login después del registro
            await Task.Delay(1000);
            await SetCurrentUserAndNavigate(newUser);

        }
        catch (Exception ex)
        {
            LoginStatus = $"Error al registrar: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SetCurrentUserAndNavigate(User user)
    {
        // Guardar usuario actual en preferencias locales
        await SecureStorage.Default.SetAsync("current_user_email", user.Email);
        await SecureStorage.Default.SetAsync("current_user_id", user.Id.ToString());

        LoginStatus = $"✅ Bienvenido, {user.Name}!";
        await Task.Delay(500);
        await Shell.Current.GoToAsync("//main");
    }

    [RelayCommand]
    private async Task UseQuickDemo()
    {
        Email = "demo@paySecure.com";
        await LoginWithEmail();
    }
}