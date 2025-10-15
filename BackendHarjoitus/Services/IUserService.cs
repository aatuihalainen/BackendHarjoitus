using BackendHarjoitus.Models;

namespace BackendHarjoitus.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<UserDTO?> GetUserAsync(string username);
        Task<User?> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(string username);
        Task<bool> UpdateUserLastLogin(long id);
    }
}
