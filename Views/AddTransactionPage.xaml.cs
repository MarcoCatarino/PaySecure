using PaySecure.ViewModels;

namespace PaySecure.Views;

[QueryProperty(nameof(TransactionId), "id")]
public partial class AddTransactionPage : ContentPage
{
    public string? TransactionId { get; set; }

    public AddTransactionPage(TransactionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Si hay un ID, cargar la transacción para editar
        if (!string.IsNullOrEmpty(TransactionId))
        {
            Title = "Editar Transacción";
        }
    }
}