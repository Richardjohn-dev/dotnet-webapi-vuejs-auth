using Auth.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Data;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }
    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        user.Id = await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _context.Users.FirstAsync(u => u.Email == email);
    }

    public async Task<User?> GetById(int id)
    {
        return await _context.Users.FindAsync(id);
    }
}
