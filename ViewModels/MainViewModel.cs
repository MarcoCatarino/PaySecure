// ViewModels/MainViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaySecure.Models;
using PaySecure.Services;
using System.Collections.ObjectModel;

namespace PaySecure.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private User? currentUser;

    [ObservableProperty]
    private decimal totalBalance;

    [ObservableProperty]
    private int totalTransactions;

    [ObservableProperty]
    private ObservableCollection<Transaction> recentTransactions = new();

    public MainViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        Title = "PaySecure";
        LoadData();
    }

    private async void LoadData()
    {
        IsBusy = true;
        try
        {
            // Cargar usuario demo
            CurrentUser = await _databaseService.GetUserByEmailAsync("demo@paySecure.com");

            if (CurrentUser != null)
            {
                // Cargar estadísticas
                TotalBalance = await _databaseService.GetTotalAmountByUserAsync(CurrentUser.Id);
                TotalTransactions = await _databaseService.GetTransactionCountByUserAsync(CurrentUser.Id);

                // Cargar transacciones recientes
                var transactions = await _databaseService.GetTransactionsAsync(CurrentUser.Id);
                RecentTransactions.Clear();
                foreach (var transaction in transactions.Take(5))
                {
                    RecentTransactions.Add(transaction);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
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
        LoadData();
    }

    [RelayCommand]
    private async Task Logout()
    {
        var result = await Shell.Current.DisplayAlert(
            "Cerrar Sesión",
            "¿Estás seguro que deseas salir?",
            "Sí", "No");

        if (result)
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}