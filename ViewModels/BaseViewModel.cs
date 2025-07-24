
// ViewModels/BaseViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;

namespace PaySecure.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;
}