using System.ComponentModel.DataAnnotations;

namespace PaySecure.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool IsBiometricEnabled { get; set; } = false;

    // Relación con transacciones
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
