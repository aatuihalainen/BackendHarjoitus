using System.ComponentModel.DataAnnotations;

namespace BackendHarjoitus.Models
{
    public class Message
    {
        public long Id { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(2000)]
        public string? Contents { get; set; }
        public User Sender { get; set; }
        public User? Recipient { get; set; }
        public Message? PrevMessage { get; set; }
    }

    public class MessageDTO
    {
        public long Id { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(2000)]
        public string? Contents { get; set; }
        public string? SenderId { get; set; }
        public string? RecipientId { get; set; }
        public long? PrevMessageId { get; set; }
    }

}
