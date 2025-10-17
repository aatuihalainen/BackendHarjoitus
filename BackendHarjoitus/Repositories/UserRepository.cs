using BackendHarjoitus.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendHarjoitus.Repositories
{
    public class UserRepository : IUserRepository
    {
        MessageContext _context;

        public UserRepository(MessageContext context)
        {
            _context = context;
        }
        public async Task<User?> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(long id)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetUserAsync(long id)
        {
            User? user = await _context.Users.FindAsync(id);
            return user;
        }

        public async Task<User?> GetUserAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users
                .OrderByDescending(i => i.Id)
                .Take(10)
                .ToListAsync();
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
