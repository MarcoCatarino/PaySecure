// Services/UserSessionService.cs - Gestión de usuario actual
using PaySecure.Models;

namespace PaySecure.Services;

public interface IUserSessionService
{
    Task<User?> GetCurrentUserAsync();
    Task SetCurrentUserAsync(User user);
    Task<int> GetCurrentUserIdAsync();
    Task ClearSessionAsync();
    Task<bool> HasActiveSessionAsync();
}

public class UserSessionService : IUserSessionService
{
    private readonly DatabaseService _databaseService;
    private User? _currentUser;

    public UserSessionService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        if (_currentUser != null)
            return _currentUser;

        try
        {
            var userEmail = await SecureStorage.Default.GetAsync("current_user_email");
            if (!string.IsNullOrEmpty(userEmail))
            {
                _currentUser = await _databaseService.GetUserByEmailAsync(userEmail);
                return _currentUser;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting current user: {ex.Message}");
        }

        return null;
    }

    public async Task SetCurrentUserAsync(User user)
    {
        _currentUser = user;
        await SecureStorage.Default.SetAsync("current_user_email", user.Email);
        await SecureStorage.Default.SetAsync("current_user_id", user.Id.ToString());
    }

    public async Task<int> GetCurrentUserIdAsync()
    {
        var user = await GetCurrentUserAsync();
        return user?.Id ?? 1; // Fallback a usuario demo
    }

    public async Task ClearSessionAsync()
    {
        _currentUser = null;
        SecureStorage.Default.Remove("current_user_email");
        SecureStorage.Default.Remove("current_user_id");
    }

    public async Task<bool> HasActiveSessionAsync()
    {
        var user = await GetCurrentUserAsync();
        return user != null;
    }
}