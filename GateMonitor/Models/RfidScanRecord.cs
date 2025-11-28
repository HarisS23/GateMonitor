using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GateMonitor.Models
{
    public class RfidScanRecord
    {
        [Key]
        public int Id { get; set; }
        [Required, ForeignKey(nameof(RfidCard))]
        public string RfidUid { get; set; } = null!;
        [ForeignKey(nameof(Worker))]
        public int? WorkerId { get; set; }
        public Worker? Worker { get; set; } = null!;
        [Required]
        public bool Success { get; set; }
        [ForeignKey(nameof(RfidScanAction))]
        public int RfidScanActionId { get; set; }
        public RfidScanAction RfidScanAction { get; set; } = null!;
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
