// MauiProgram.cs - Con UserSessionService
using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaySecure.Data;
using PaySecure.Services;
using PaySecure.ViewModels;
using PaySecure.Views;

namespace PaySecure;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Configurar base de datos
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "paySecure.db");
            options.UseSqlite($"Data Source={dbPath}");
        });

        // Registrar servicios
        builder.Services.AddSingleton<IBiometricService, BiometricService>();
        builder.Services.AddSingleton<IPaymentService, PaymentService>();
        builder.Services.AddSingleton<IUserSessionService, UserSessionService>(); // NUEVO
        builder.Services.AddScoped<DatabaseService>();

        // Registrar ViewModels
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<TransactionViewModel>();
        builder.Services.AddTransient<PaymentViewModel>();

        // Registrar Views
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<TransactionListPage>();
        builder.Services.AddTransient<AddTransactionPage>();
        builder.Services.AddTransient<PaymentPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Inicializar base de datos
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
        }

        return app;
    }
}