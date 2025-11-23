using GateMonitor.Controllers.Hubs;
using GateMonitor.Data;
using GateMonitor.Models;
using GateMonitor.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace GateMonitor.Controllers
{
    [ApiController]
    [Route("api/rfid")]
    public class RfidController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<RfidScanHub> _hub;

        public RfidController(AppDbContext db, IHubContext<RfidScanHub> hub)
        {
            _db = db;
            _hub = hub;
        }

        [HttpPost("log")]
        public async Task<IActionResult> LogScan([FromBody] RfidScanRequest request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrEmpty(request.RfidCardUid))
            {
                return BadRequest("Invalid request data");
            }
            
            var worker = await _db.Workers.FirstOrDefaultAsync(w => w.RfidUid == request.RfidCardUid, cancellationToken);
            
            var log = new RfidScanRecord
            {
                WorkerId = worker?.Id,
                RfidUid = request.RfidCardUid,
                RfidScanActionId = request.RfidScanActionId,
                Success = worker is { HasAccess: true },
                CreatedAt = DateTime.UtcNow
            };

            await _db.RfidScanRecords.AddAsync(log, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            await _hub.Clients.All.SendAsync("NewRfidScan", new
            {
                log.Id,
                Worker = worker != null ? new { worker.FirstName, worker.LastName } : null,
                log.RfidUid,
                RfidScanAction = log.RfidScanActionId != 0
                    ? (await _db.RfidScanActions
                        .Where(a => a.Id == log.RfidScanActionId)
                        .Select(a => a.PrettyName)
                        .FirstOrDefaultAsync(cancellationToken))
                    : null,
                log.Success,
                CreatedAt = log.CreatedAt
            }, cancellationToken);

            return Ok(new
            {
                success = worker is { HasAccess: true },
                message = "Scan logged",
                worker = worker != null ? worker.FirstName + " " + worker.LastName : null
            });
        }
    }
}
