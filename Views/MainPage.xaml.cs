using PaySecure.ViewModels;

namespace PaySecure.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Inicializar datos cada vez que la página aparece
        await _viewModel.InitializeAsync();
    }
}