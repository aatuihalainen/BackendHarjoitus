using BackendHarjoitus.Models;
using BackendHarjoitus.Repositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Drawing.Printing;
using System.Security.Cryptography;
using System.Security.Permissions;

namespace BackendHarjoitus.Middleware
{
    public interface IUserAuthenticationService
    {
        Task<User?> Authenticate(string username, string password);
        public User CreateUserCredentials(User user);
        Task<bool> IsMyMessage(string username, long messageId);
        Task<bool> CanUserViewMessage(string username, long messageId);
    }
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public UserAuthenticationService(IUserRepository userRepository, IMessageRepository messageRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }
        
        public async Task<User?> Authenticate(string username, string password)
        {
            User? user;
            user = await _userRepository.GetUserAsync(username);
            if (user == null)
            {
                return null;
            }
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: password, salt: user.Salt, prf: KeyDerivationPrf.HMACSHA256, iterationCount: 10000, numBytesRequested: 258 / 8));

            if (hashedPassword != user.Password)
            {
                return null;
            }

            return user;
        }

        public User CreateUserCredentials(User user)
        {
            byte[] salt = new byte[128/8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: user.Password, salt: salt, prf: KeyDerivationPrf.HMACSHA256, iterationCount: 10000, numBytesRequested: 258/8));
            
            user.Password = hashedPassword;
            user.Salt = salt;
            return user;


        }

        public async Task<bool> IsMyMessage(string username, long messageId)
        {
            User? user = await _userRepository.GetUserAsync(username);
            if (user == null)
            {
                return false;
            }

            Message? message = await _messageRepository.GetMessageAsync(messageId);
            if (message == null)
            {
                return false;
            }

            if( message.Sender == user)
            {
                return true;
            } 

            return false;
        }

        public async Task<bool> CanUserViewMessage(string username, long messageId)
        {
            User? user = await _userRepository.GetUserAsync(username);
            if (user == null)
            {
                return false;
            }

            Message? message = await _messageRepository.GetMessageAsync(messageId);
            if (message == null)
            {
                return false;
            }

            if (message.Recipient == null || message.Recipient == user || message.Sender == user)
            {
                return true;
            }
            return false;
        }
    }
}
