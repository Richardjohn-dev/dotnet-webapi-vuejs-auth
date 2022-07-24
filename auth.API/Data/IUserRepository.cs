using Auth.API.Models;

namespace Auth.API.Data;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User?> GetByEmail(string email);
    Task<User?> GetById(int id);
}
