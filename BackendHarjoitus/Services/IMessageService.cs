using BackendHarjoitus.Models;
namespace BackendHarjoitus.Services
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDTO>> GetMessagesAsync();
        Task<MessageDTO?> GetMessageAsync(long id);
        Task<MessageDTO?> CreateMessageAsync(MessageDTO message);
        Task<bool> UpdateMessageAsync(MessageDTO message);
        Task<bool> DeleteMessageAsync(long id);
    }
}
