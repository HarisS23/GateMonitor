using System.ComponentModel.DataAnnotations;

namespace GateMonitor.Models
{
    //Machine Turned On
    //Machine Turned Off
    public class RfidScanAction
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string PrettyName { get; set; } = null!;
    }
}
