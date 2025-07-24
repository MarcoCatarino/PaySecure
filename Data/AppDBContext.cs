// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using PaySecure.Models;

namespace PaySecure.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar relaciones
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.User)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurar precisión decimal
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        // Índices para optimización
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.CreatedAt);

        // Datos de prueba
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Usuario Demo",
                Email = "demo@paySecure.com",
                Phone = "+52 55 1234 5678",
                CreatedAt = DateTime.Now,
                IsBiometricEnabled = true
            }
        );
    }
}