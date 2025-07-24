using Stripe;
using PaySecure.Models;

namespace PaySecure.Services;

public interface IPaymentService
{
    Task<PaymentResult> CreatePaymentIntentAsync(PaymentRequest request);
    Task<PaymentResult> ConfirmPaymentAsync(string paymentIntentId);
    Task<PaymentResult> RefundPaymentAsync(string paymentIntentId, decimal? amount = null);
    Task<List<PaymentMethodInfo>> GetPaymentMethodsAsync(string customerId);
}

public class PaymentService : IPaymentService
{
    private readonly PaymentIntentService _paymentIntentService;
    private readonly RefundService _refundService;
    private readonly PaymentMethodService _paymentMethodService;

    public PaymentService()
    {
        //TODO: IMPORTANTE: En producción, usar variables de entorno
        StripeConfiguration.ApiKey = "sk_test_tu_clave_secreta_aqui";

        _paymentIntentService = new PaymentIntentService();
        _refundService = new RefundService();
        _paymentMethodService = new PaymentMethodService();
    }

    public async Task<PaymentResult> CreatePaymentIntentAsync(PaymentRequest request)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100), // Stripe usa centavos
                Currency = request.Currency,
                Description = request.Description,
                Metadata = request.Metadata,
                PaymentMethodTypes = new List<string> { "card" },
                ReceiptEmail = request.CustomerEmail
            };

            var paymentIntent = await _paymentIntentService.CreateAsync(options);

            return new PaymentResult
            {
                Success = true,
                PaymentIntentId = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret,
                Status = paymentIntent.Status,
                Message = "Payment intent created successfully"
            };
        }
        catch (StripeException ex)
        {
            return new PaymentResult
            {
                Success = false,
                Message = $"Stripe Error: {ex.Message}",
                ErrorCode = ex.StripeError?.Code
            };
        }
        catch (Exception ex)
        {
            return new PaymentResult
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }

    public async Task<PaymentResult> ConfirmPaymentAsync(string paymentIntentId)
    {
        try
        {
            var paymentIntent = await _paymentIntentService.GetAsync(paymentIntentId);

            return new PaymentResult
            {
                Success = paymentIntent.Status == "succeeded",
                PaymentIntentId = paymentIntent.Id,
                Status = paymentIntent.Status,
                Amount = paymentIntent.Amount / 100m, // Convertir de centavos
                Message = paymentIntent.Status == "succeeded" ?
                    "Payment completed successfully" :
                    $"Payment status: {paymentIntent.Status}"
            };
        }
        catch (StripeException ex)
        {
            return new PaymentResult
            {
                Success = false,
                Message = $"Stripe Error: {ex.Message}",
                ErrorCode = ex.StripeError?.Code
            };
        }
    }

    public async Task<PaymentResult> RefundPaymentAsync(string paymentIntentId, decimal? amount = null)
    {
        try
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };

            if (amount.HasValue)
                options.Amount = (long)(amount.Value * 100);

            var refund = await _refundService.CreateAsync(options);

            return new PaymentResult
            {
                Success = refund.Status == "succeeded",
                RefundId = refund.Id,
                Amount = refund.Amount / 100m,
                Status = refund.Status,
                Message = refund.Status == "succeeded" ?
                    "Refund processed successfully" :
                    $"Refund status: {refund.Status}"
            };
        }
        catch (StripeException ex)
        {
            return new PaymentResult
            {
                Success = false,
                Message = $"Stripe Error: {ex.Message}",
                ErrorCode = ex.StripeError?.Code
            };
        }
    }

    public async Task<List<PaymentMethodInfo>> GetPaymentMethodsAsync(string customerId)
    {
        try
        {
            var options = new PaymentMethodListOptions
            {
                Customer = customerId,
                Type = "card"
            };

            var paymentMethods = await _paymentMethodService.ListAsync(options);

            return paymentMethods.Data.Select(pm => new PaymentMethodInfo
            {
                Id = pm.Id,
                Brand = pm.Card?.Brand ?? "unknown",
                Last4 = pm.Card?.Last4 ?? "****",
                ExpiryMonth = pm.Card?.ExpMonth ?? 0,
                ExpiryYear = pm.Card?.ExpYear ?? 0
            }).ToList();
        }
        catch (Exception)
        {
            return new List<PaymentMethodInfo>();
        }
    }
}

// Modelos auxiliares
public class PaymentResult
{
    public bool Success { get; set; }
    public string PaymentIntentId { get; set; } = string.Empty;
    public string RefundId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public decimal Amount { get; set; }
}

public class PaymentMethodInfo
{
    public string Id { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Last4 { get; set; } = string.Empty;
    public long ExpiryMonth { get; set; }
    public long ExpiryYear { get; set; }
}