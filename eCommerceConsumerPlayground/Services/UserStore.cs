using eCommerceConsumerPlayground.Models;
using ECommerceConsumerPlayground.Models;
using ECommerceConsumerPlayground.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerceConsumerPlayground.Services;

/// <summary>
/// Implementation of User objects in database
/// </summary>
public class UserStore : IUserStore
{
    private readonly ILogger<UserStore> _logger;
    private readonly AppDbContext _context;

    public UserStore(ILogger<UserStore> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public async Task SaveDataAsync(User user)
    {
        _logger.LogInformation($"Starting persistence operations for user object '{user}' in database.");
        try
        {
            // Check if entry already exists
            var userExists = await CheckIfEntryAlreadyExistsAsync(user);
            if (userExists)
            {
                _logger.LogInformation($"User object '{user.Username}' already exists in database. No new persistence.");
                return;
            }



            // If not already exists, than persist
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            
        }
        catch (Exception e)
        {
            _logger.LogError($"User object '{user}' could not be saved on database. Message: {e.Message}");
        }
    }

    private async Task<bool> CheckIfEntryAlreadyExistsAsync(User user)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Username == user.Username);
        return userExists;
    }
}