using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GateMonitor.Models
{
    public class Worker
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required]
        public string RfidUid { get; set; } = null!;
        [Required]
        public bool HasAccess { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public ICollection<RfidScanRecord> RfidScanRecords { get; set; } = new List<RfidScanRecord>();

    }
}
