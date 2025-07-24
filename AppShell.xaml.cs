namespace PaySecure;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("addtransaction", typeof(Views.AddTransactionPage));
    }
}