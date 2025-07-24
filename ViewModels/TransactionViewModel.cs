// ViewModels/TransactionViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaySecure.Models;
using PaySecure.Services;
using Stripe.V2;
using System.Collections.ObjectModel;

namespace PaySecure.ViewModels;

public partial class TransactionViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<Transaction> transactions = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private TransactionType selectedType = TransactionType.Payment;

    // Para agregar nueva transacción
    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private decimal amount;

    [ObservableProperty]
    private bool isAddingTransaction;

    public List<TransactionType> TransactionTypes { get; } =
        Enum.GetValues<TransactionType>().ToList();

    public TransactionViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        Title = "Transacciones";
        LoadTransactions();
    }

    private async void LoadTransactions()
    {
        IsBusy = true;
        try
        {
            var allTransactions = await _databaseService.GetTransactionsAsync();
            Transactions.Clear();

            foreach (var transaction in allTransactions)
            {
                Transactions.Add(transaction);
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
    private void ShowAddTransaction()
    {
        IsAddingTransaction = true;
        Description = string.Empty;
        Amount = 0;
        SelectedType = TransactionType.Payment;
    }

    [RelayCommand]
    private void CancelAddTransaction()
    {
        IsAddingTransaction = false;
    }

    [RelayCommand]
    private async Task SaveTransaction()
    {
        if (string.IsNullOrWhiteSpace(Description) || Amount <= 0)
        {
            await Shell.Current.DisplayAlert("Error",
                "Por favor completa todos los campos correctamente", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            var transaction = new Transaction
            {
                UserId = 1, // Usuario demo
                Description = Description,
                Amount = Amount,
                Type = SelectedType,
                Status = TransactionStatus.Pending
            };

            await _databaseService.CreateTransactionAsync(transaction);
            IsAddingTransaction = false;
            LoadTransactions();

            await Shell.Current.DisplayAlert("Éxito",
                "Transacción creada correctamente", "OK");
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
    private async Task DeleteTransaction(Transaction transaction)
    {
        var result = await Shell.Current.DisplayAlert(
            "Eliminar",
            $"¿Eliminar la transacción '{transaction.Description}'?",
            "Sí", "No");

        if (result)
        {
            try
            {
                await _databaseService.DeleteTransactionAsync(transaction.Id);
                Transactions.Remove(transaction);
                await Shell.Current.DisplayAlert("Éxito",
                    "Transacción eliminada", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    [RelayCommand]
    private async Task EditTransaction(Transaction transaction)
    {
        // Navegar a página de edición con parámetros
        await Shell.Current.GoToAsync($"addtransaction?id={transaction.Id}");
    }

    [RelayCommand]
    private async Task SearchTransactions()
    {
        // Implementar búsqueda si es necesario
        LoadTransactions();
    }
}