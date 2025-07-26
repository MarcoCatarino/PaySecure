// ViewModels/MainViewModel.cs - Mejorado con carga de datos
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaySecure.Models;
using PaySecure.Services;
using System.Collections.ObjectModel;

namespace PaySecure.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly IUserSessionService _userSessionService;

    [ObservableProperty]
    private User? currentUser;

    [ObservableProperty]
    private decimal totalBalance;

    [ObservableProperty]
    private int totalTransactions;

    [ObservableProperty]
    private ObservableCollection<Transaction> recentTransactions = new();

    [ObservableProperty]
    private string welcomeMessage = "Bienvenido a PaySecure";

    [ObservableProperty]
    private bool hasUserData = false;

    public MainViewModel(DatabaseService databaseService, IUserSessionService userSessionService)
    {
        _databaseService = databaseService;
        _userSessionService = userSessionService;
        Title = "PaySecure";
    }

    public async Task InitializeAsync()
    {
        IsBusy = true;
        try
        {
            await LoadCurrentUser();
            await LoadUserData();
            HasUserData = CurrentUser != null;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error al cargar datos", ex.Message, "OK");
            System.Diagnostics.Debug.WriteLine($"Error in InitializeAsync: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadCurrentUser()
    {
        try
        {
            CurrentUser = await _userSessionService.GetCurrentUserAsync();

            if (CurrentUser != null)
            {
                WelcomeMessage = $"¡Hola, {CurrentUser.Name}!";
                Title = $"PaySecure - {CurrentUser.Name}";
                System.Diagnostics.Debug.WriteLine($"✅ Usuario cargado: {CurrentUser.Name} ({CurrentUser.Email})");
            }
            else
            {
                // Fallback al usuario demo si no hay sesión
                System.Diagnostics.Debug.WriteLine("⚠️ No hay usuario en sesión, cargando demo...");
                CurrentUser = await _databaseService.GetUserByEmailAsync("demo@paySecure.com");
                if (CurrentUser != null)
                {
                    WelcomeMessage = $"¡Hola, {CurrentUser.Name}!";
                    await _userSessionService.SetCurrentUserAsync(CurrentUser);
                }
                else
                {
                    WelcomeMessage = "Error: Usuario no encontrado";
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error loading current user: {ex.Message}");
            WelcomeMessage = "Error al cargar usuario";
        }
    }

    private async Task LoadUserData()
    {
        if (CurrentUser == null)
        {
            System.Diagnostics.Debug.WriteLine("❌ No hay usuario actual para cargar datos");
            return;
        }

        try
        {
            System.Diagnostics.Debug.WriteLine($"🔄 Cargando datos para usuario ID: {CurrentUser.Id}");

            // Cargar estadísticas del usuario actual
            TotalBalance = await _databaseService.GetTotalAmountByUserAsync(CurrentUser.Id);
            TotalTransactions = await _databaseService.GetTransactionCountByUserAsync(CurrentUser.Id);

            System.Diagnostics.Debug.WriteLine($"📊 Balance: ${TotalBalance}, Transacciones: {TotalTransactions}");

            // Cargar transacciones recientes del usuario actual
            var transactions = await _databaseService.GetTransactionsAsync(CurrentUser.Id);
            RecentTransactions.Clear();

            foreach (var transaction in transactions.Take(5))
            {
                RecentTransactions.Add(transaction);
            }

            System.Diagnostics.Debug.WriteLine($"📝 Transacciones recientes cargadas: {RecentTransactions.Count}");

            // Forzar actualización de propiedades observables
            OnPropertyChanged(nameof(CurrentUser));
            OnPropertyChanged(nameof(TotalBalance));
            OnPropertyChanged(nameof(TotalTransactions));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error loading user data: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", $"Error al cargar datos del usuario: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task GoToTransactions()
    {
        await Shell.Current.GoToAsync("//transactions");
    }

    [RelayCommand]
    private async Task GoToPayments()
    {
        await Shell.Current.GoToAsync("//payment");
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await InitializeAsync();
    }

    [RelayCommand]
    private async Task Logout()
    {
        var result = await Shell.Current.DisplayAlert(
            "Cerrar Sesión",
            $"¿Estás seguro que deseas salir, {CurrentUser?.Name}?",
            "Sí", "No");

        if (result)
        {
            await _userSessionService.ClearSessionAsync();
            await Shell.Current.GoToAsync("//login");
        }
    }

    [RelayCommand]
    private async Task SwitchUser()
    {
        var result = await Shell.Current.DisplayAlert(
            "Cambiar Usuario",
            "¿Deseas cambiar a otro usuario?",
            "Sí", "No");

        if (result)
        {
            await Shell.Current.GoToAsync("//login");
        }
    }

    [RelayCommand]
    private async Task ShowDatabaseInfo()
    {
        if (CurrentUser == null) return;

        try
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "paySecure.db");
            var allUsers = await _databaseService.GetUsersAsync();
            var userTransactions = await _databaseService.GetTransactionsAsync(CurrentUser.Id);

            var info = $"📊 Información de la Base de Datos\n\n" +
                      $"👤 Usuario Actual: {CurrentUser.Name}\n" +
                      $"📧 Email: {CurrentUser.Email}\n" +
                      $"📱 Teléfono: {CurrentUser.Phone}\n" +
                      $"🔒 Biometría: {(CurrentUser.IsBiometricEnabled ? "Habilitada" : "Deshabilitada")}\n\n" +
                      $"📊 Estadísticas:\n" +
                      $"• Total usuarios: {allUsers.Count}\n" +
                      $"• Transacciones del usuario: {userTransactions.Count}\n" +
                      $"• Balance: ${TotalBalance:F2}\n\n" +
                      $"📁 BD: {dbPath}";

            await Shell.Current.DisplayAlert("Info de Usuario", info, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}