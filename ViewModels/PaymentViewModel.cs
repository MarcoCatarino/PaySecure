// ViewModels/PaymentViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaySecure.Models;
using PaySecure.Services;
using Stripe;
using Stripe.V2;

namespace PaySecure.ViewModels;

public partial class PaymentViewModel : BaseViewModel
{
    private readonly IPaymentService _paymentService;
    private readonly DatabaseService _databaseService;
    private readonly IBiometricService _biometricService;

    [ObservableProperty]
    private decimal amount = 100;

    [ObservableProperty]
    private string description = "Pago de prueba";

    [ObservableProperty]
    private string customerEmail = "cliente@ejemplo.com";

    [ObservableProperty]
    private string paymentStatus = string.Empty;

    [ObservableProperty]
    private string paymentIntentId = string.Empty;

    [ObservableProperty]
    private bool isProcessingPayment;

    public PaymentViewModel(IPaymentService paymentService,
                          DatabaseService databaseService,
                          IBiometricService biometricService)
    {
        _paymentService = paymentService;
        _databaseService = databaseService;
        _biometricService = biometricService;
        Title = "Realizar Pago";
    }

    [RelayCommand]
    private async Task ProcessPayment()
    {
        if (Amount <= 0 || string.IsNullOrWhiteSpace(Description))
        {
            PaymentStatus = "❌ Por favor completa todos los campos";
            return;
        }

        // Primero autenticar con biometría
        var biometricAuth = await _biometricService.AuthenticateAsync(
            $"Confirma el pago de ${Amount:F2} MXN");

        if (!biometricAuth)
        {
            PaymentStatus = "❌ Autenticación biométrica fallida";
            return;
        }

        IsProcessingPayment = true;
        PaymentStatus = "🔄 Procesando pago...";

        try
        {
            var paymentRequest = new PaymentRequest
            {
                Amount = Amount,
                Currency = "mxn",
                Description = Description,
                CustomerEmail = CustomerEmail,
                Metadata = new Dictionary<string, string>
                {
                    {"app", "PaySecure"},
                    {"user_id", "1"}
                }
            };

            var result = await _paymentService.CreatePaymentIntentAsync(paymentRequest);

            if (result.Success)
            {
                PaymentIntentId = result.PaymentIntentId;
                PaymentStatus = "✅ Pago creado exitosamente";

                // Crear transacción en la base de datos
                var transaction = new Transaction
                {
                    UserId = 1,
                    Description = Description,
                    Amount = Amount,
                    Type = TransactionType.Payment,
                    Status = TransactionStatus.Completed,
                    StripePaymentIntentId = result.PaymentIntentId,
                    CompletedAt = DateTime.Now
                };

                await _databaseService.CreateTransactionAsync(transaction);

                await Shell.Current.DisplayAlert("Éxito",
                    $"Pago de ${Amount:F2} MXN procesado correctamente", "OK");

                // Limpiar formulario
                Amount = 0;
                Description = string.Empty;
                CustomerEmail = string.Empty;
            }
            else
            {
                PaymentStatus = $"❌ Error: {result.Message}";
                await Shell.Current.DisplayAlert("Error de Pago", result.Message, "OK");
            }
        }
        catch (Exception ex)
        {
            PaymentStatus = $"❌ Error: {ex.Message}";
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsProcessingPayment = false;
        }
    }

    [RelayCommand]
    private async Task RefundPayment()
    {
        if (string.IsNullOrWhiteSpace(PaymentIntentId))
        {
            await Shell.Current.DisplayAlert("Error",
                "No hay un pago para reembolsar", "OK");
            return;
        }

        var confirm = await Shell.Current.DisplayAlert("Reembolso",
            $"¿Reembolsar el pago de ${Amount:F2} MXN?", "Sí", "No");

        if (!confirm) return;

        IsProcessingPayment = true;
        PaymentStatus = "🔄 Procesando reembolso...";

        try
        {
            var result = await _paymentService.RefundPaymentAsync(PaymentIntentId);

            if (result.Success)
            {
                PaymentStatus = "✅ Reembolso procesado";

                // Crear transacción de reembolso
                var refundTransaction = new Transaction
                {
                    UserId = 1,
                    Description = $"Reembolso: {Description}",
                    Amount = -Amount,
                    Type = TransactionType.Refund,
                    Status = TransactionStatus.Completed,
                    StripePaymentIntentId = result.RefundId,
                    CompletedAt = DateTime.Now
                };

                await _databaseService.CreateTransactionAsync(refundTransaction);

                await Shell.Current.DisplayAlert("Éxito",
                    "Reembolso procesado correctamente", "OK");
            }
            else
            {
                PaymentStatus = $"❌ Error en reembolso: {result.Message}";
            }
        }
        catch (Exception ex)
        {
            PaymentStatus = $"❌ Error: {ex.Message}";
        }
        finally
        {
            IsProcessingPayment = false;
        }
    }
}
