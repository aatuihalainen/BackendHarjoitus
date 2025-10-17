using BackendHarjoitus.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendHarjoitus.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        MessageContext _context;
        public MessageRepository(MessageContext context)
        {
            _context = context;
        }

        public async Task<Message?> CreateMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<bool> DeleteMessageAsync(long id)
        {
            Message? message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return false;
            }

            message.Title = "Deleted";
            message.Contents = "Message has been deleted";

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

        public async Task<Message?> GetMessageAsync(long id)
        {
            Message? message = await _context.Messages.Include(i => i.Sender).Include(i => i.Recipient).FirstOrDefaultAsync(i => i.Id == id);
            return message;
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync()
        {
            return await _context.Messages
                .Include(i => i.Sender)
                .Include(i => i.Recipient)
                .Where(i => i.Recipient == null)
                .OrderByDescending(i => i.Id)
                .Take(10)
                .ToListAsync();
        }

        public async Task<bool> UpdateMessageAsync(Message message)
        {
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
