using System.ComponentModel.DataAnnotations;

namespace GateMonitor.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string PasswordHash { get; set; } = null!;
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
