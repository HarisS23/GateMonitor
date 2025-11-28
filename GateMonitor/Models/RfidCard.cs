using System.ComponentModel.DataAnnotations;

namespace GateMonitor.Models
{
    public class RfidCard
    {
        [Key]
        public string Uid { get; set; } = null!;
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
