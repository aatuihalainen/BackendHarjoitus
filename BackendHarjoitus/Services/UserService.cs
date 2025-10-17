
using BackendHarjoitus.Middleware;
using BackendHarjoitus.Models;
using BackendHarjoitus.Repositories;
using Microsoft.OpenApi.Any;
using NuGet.Protocol;
using System.Runtime.ConstrainedExecution;

namespace BackendHarjoitus.Services
{
    public class UserService : IUserService
    {
        IUserRepository _repository;
        readonly IUserAuthenticationService _authenticationService;

        public UserService(IUserRepository repository, IUserAuthenticationService authService)
        {
            _repository = repository;
            _authenticationService = authService;
        }

        public async Task<User?> CreateUserAsync(User user)
        {
            User? dbUser = await _repository.GetUserAsync(user.Username);
            if (dbUser != null)
            {
                return null;
            }

            user.CreatedDate = DateTime.Now;
            user.LastLogin = DateTime.Now;

            User? newUser = _authenticationService.CreateUserCredentials(user);
            if (newUser != null)
            {
                return await _repository.CreateUserAsync(newUser);

            }
            return null;
        }

        public async Task<bool> DeleteUserAsync(string username)
        {
            User? user = await _repository.GetUserAsync(username);
            if (user == null)
            {
                return false;
            }
            return await _repository.DeleteUserAsync(user.Id);
        }

        public async Task<UserDTO?> GetUserAsync(string username)
        {
            User? user = await _repository.GetUserAsync(username);
            if (user == null)
            {
                return null;
            }
            return UserToDTO(user);
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            IEnumerable<User> users = await _repository.GetUsersAsync();

            List<UserDTO> result = new List<UserDTO>();
            foreach (User user in users) 
            {
                result.Add(UserToDTO(user));                
            }

            return result;

        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            User? dbUser = await _repository.GetUserAsync(user.Username);
            if (dbUser == null)
            {
                return false;
            }

            _authenticationService.CreateUserCredentials(user);
            dbUser.Salt = user.Salt;
            dbUser.Password = user.Password;
            dbUser.FirstName = user.FirstName;
            dbUser.LastName = user.LastName;

            return await _repository.UpdateUserAsync(dbUser);
        }

        public async Task<bool> UpdateUserLastLogin(long id)
        {
            User? dbUser = await _repository.GetUserAsync(id);
            if (dbUser == null)
            {
                return false;
            }
            dbUser.LastLogin = DateTime.Now;
            return await _repository.UpdateUserAsync(dbUser);
        }

        private UserDTO UserToDTO(User user)
        {
            UserDTO dto = new UserDTO();
            dto.Username = user.Username;
            dto.FirstName = user.FirstName;
            dto.LastName = user.LastName;
            dto.CreatedDate = user.CreatedDate;
            dto.LastLogin = user.LastLogin;

            return dto;
        }
    }
}
