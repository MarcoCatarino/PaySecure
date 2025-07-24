namespace PaySecure.Models;

public class PaymentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "mxn";
    public string Description { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
}