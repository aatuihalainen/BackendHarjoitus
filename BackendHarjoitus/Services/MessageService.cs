using BackendHarjoitus.Models;
using BackendHarjoitus.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BackendHarjoitus.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;
        private readonly IUserRepository _userRepository;

        public MessageService(IMessageRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<MessageDTO?> CreateMessageAsync(MessageDTO message)
        {
            return MessageToDTO(await _repository.CreateMessageAsync(await DTOToMessageAsync(message)));
        }

        public async Task<bool> DeleteMessageAsync(long id)
        {
            Message? message = await _repository.GetMessageAsync(id);
            if (message == null)
            {
                return false;
            }



            return await _repository.DeleteMessageAsync(id);
        }

        public async Task<MessageDTO?> GetMessageAsync(long id)
        {
            return MessageToDTO (await _repository.GetMessageAsync(id));
        }

        public async Task<IEnumerable<MessageDTO>> GetMessagesAsync()
        {
            IEnumerable<Message> messages = await _repository.GetMessagesAsync();

            List<MessageDTO> result = new List<MessageDTO>();
            foreach (Message msg in messages)
            {
                result.Add(MessageToDTO(msg));
            }

            return result;
        }

        public async Task<bool> UpdateMessageAsync(MessageDTO message)
        {
            Message? oldMessage = await _repository.GetMessageAsync(message.Id);
            if(oldMessage == null)
            {
                return false;
            }

            return await _repository.UpdateMessageAsync(await DTOToMessageAsync(message));
        }

        private MessageDTO? MessageToDTO(Message message)
        {
            if (message == null)
            {
                return null;
            }

            MessageDTO dto = new MessageDTO();
            dto.Id = message.Id;
            dto.Title = message.Title;
            dto.Contents = message.Contents;
            dto.SenderId = message.Sender.Username;

            if (message.Recipient != null)
            {
                dto.RecipientId = message.Recipient.Username;
            }

            if (message.PrevMessage != null)
            {
                dto.PrevMessageId = message.PrevMessage.Id;
            }

            return dto;
        }

        private async Task<Message> DTOToMessageAsync(MessageDTO dto)
        {
            Message msg = new Message();
            msg.Id = dto.Id;
            msg.Title = dto.Title;
            msg.Contents = dto.Contents;
            
            User? sender = await _userRepository.GetUserAsync(dto.SenderId);
            if (sender != null)
            {
                msg.Sender = sender;
            }

            if (dto.RecipientId != null)
            {
                User? recipient = await _userRepository.GetUserAsync(dto.RecipientId);

                if (recipient != null)
                {
                    msg.Recipient = recipient;
                }
            }

            if (dto.PrevMessageId != null && dto.PrevMessageId != 0)
            {
                Message prevMessage = await _repository.GetMessageAsync((long)dto.PrevMessageId);
                msg.PrevMessage = prevMessage;
            }
            return msg;
        }
    }
}
