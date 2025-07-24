using Microsoft.EntityFrameworkCore;
using PaySecure.Data;
using PaySecure.Models;

namespace PaySecure.Services;

public class DatabaseService
{
    private readonly AppDbContext _context;

    public DatabaseService(AppDbContext context)
    {
        _context = context;
    }

    // CRUD para Users
    public async Task<List<User>> GetUsersAsync()
    {
        return await _context.Users.Include(u => u.Transactions).ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Transactions)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Transactions)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<int> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user.Id;
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    // CRUD para Transactions
    public async Task<List<Transaction>> GetTransactionsAsync(int? userId = null)
    {
        var query = _context.Transactions.Include(t => t.User).AsQueryable();

        if (userId.HasValue)
            query = query.Where(t => t.UserId == userId.Value);

        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    public async Task<Transaction?> GetTransactionByIdAsync(int id)
    {
        return await _context.Transactions
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<int> CreateTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction.Id;
    }

    public async Task UpdateTransactionAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTransactionAsync(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction != null)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }

    // Métodos de análisis
    public async Task<decimal> GetTotalAmountByUserAsync(int userId)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId && t.Status == TransactionStatus.Completed)
            .SumAsync(t => t.Amount);
    }

    public async Task<int> GetTransactionCountByUserAsync(int userId)
    {
        return await _context.Transactions
            .CountAsync(t => t.UserId == userId);
    }
}