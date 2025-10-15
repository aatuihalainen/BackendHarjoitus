using System.ComponentModel.DataAnnotations;

namespace BackendHarjoitus.Models
{
    public class User
    {
        public long Id { get; set; }
        [MinLength(3)]
        [MaxLength(30)]
        public string Username { get; set; }
        [MinLength(5)]
        public string Password { get; set; }
        public byte[]? Salt { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastLogin { get; set; }
    }

    public class UserDTO
    {
        [MinLength(3)]
        [MaxLength(30)]
        public string Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
