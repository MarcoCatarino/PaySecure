using PaySecure.ViewModels;

namespace PaySecure.Views;

public partial class TransactionListPage : ContentPage
{
    public TransactionListPage(TransactionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}