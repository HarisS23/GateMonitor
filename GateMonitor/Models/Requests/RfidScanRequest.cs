using GateMonitor.Models.Enums;
using Microsoft.EntityFrameworkCore.Storage;

namespace GateMonitor.Models.Requests
{
    public class RfidScanRequest
    {
        public string RfidCardUid { get; set; } = null!;
        public int RfidScanActionId { get; set; }
    }
}
