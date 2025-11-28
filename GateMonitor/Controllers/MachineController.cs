using GateMonitor.Controllers.Hubs;
using GateMonitor.Data;
using GateMonitor.Models;
using GateMonitor.Models.Requests;
using GateMonitor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using GateMonitor.Controllers.WebSockets;
using GateMonitor.Models.Enums;
using WebSocketManager = GateMonitor.Controllers.WebSockets.WebSocketManager;
using Azure.Core;
using System.Threading;

namespace GateMonitor.Controllers
{
    [ApiController]
    [Route("api/machine")]
    public class MachineController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<RfidScanHub> _hub;
        private readonly WebSocketManager _wsManager;

        public MachineController(AppDbContext db, IHubContext<RfidScanHub> hub, WebSocketManager wsManager)
        {
            _db = db;
            _hub = hub;
            _wsManager = wsManager;
        }

        [HttpPost("log")]
        public async Task<IActionResult> LogScan([FromBody] RfidScanRequest request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrEmpty(request.RfidCardUid))
            {
                return BadRequest("Invalid request data");
            }

            var rfidCard = await _db.RfidCards.FirstOrDefaultAsync(rc => rc.Uid == request.RfidCardUid, cancellationToken);

            if (rfidCard == null)
            {
                await _db.AddAsync(new RfidCard { Uid = request.RfidCardUid, CreatedAt = DateTime.UtcNow },
                    cancellationToken);

                await _db.SaveChangesAsync(cancellationToken);
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

            if (log.Success)
            {
                MachineStateService.IsMachineOn = request.RfidScanActionId == (int)RfidScanActionEnum.MachineTurnedOn;
            }

            return Ok(new
            {
                success = worker is { HasAccess: true },
                message = "Scan logged",
                worker = worker != null ? worker.FirstName + " " + worker.LastName : null
            });
        }
        
        [HttpPost("state")]
        public IActionResult SetMachineState([FromBody] MachineStateRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data");
            }
            MachineStateService.IsMachineOn = request.IsMachineOn;
            return Ok(new
            {
                success = true,
                message = $"Machine state set to {request.IsMachineOn}"
            });
        }

        [Authorize]
        [HttpPost("toggle")]
        public async Task<IActionResult> Toggle(CancellationToken cancellationToken)
        {
            MachineStateService.IsMachineOn = !MachineStateService.IsMachineOn;

            var rfidCard = await _db.RfidCards.FirstOrDefaultAsync(rc => rc.Uid == "admin", cancellationToken);

            if (rfidCard == null)
            {
                return NotFound("Admin RFID card not found");
            }

            var log = new RfidScanRecord
            {
                WorkerId = null,
                RfidUid = rfidCard.Uid,
                RfidScanActionId = MachineStateService.IsMachineOn ? (int)RfidScanActionEnum.MachineTurnedOn : (int)RfidScanActionEnum.MachineTurnedOff,
                Success = true,
                CreatedAt = DateTime.UtcNow
            };

            await _db.RfidScanRecords.AddAsync(log, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            await _hub.Clients.All.SendAsync("NewRfidScan", new
            {
                log.Id,
                Worker = "",
                RfidUid = "admin",
                RfidScanAction = log.RfidScanActionId != 0
                    ? (await _db.RfidScanActions
                        .Where(a => a.Id == log.RfidScanActionId)
                        .Select(a => a.PrettyName)
                        .FirstOrDefaultAsync())
                    : null,
                log.Success,
                CreatedAt = log.CreatedAt
            }, cancellationToken);

            var stateMsg = MachineStateService.IsMachineOn ? "ON" : "OFF";
            await _wsManager.BroadcastMessageAsync(stateMsg);

            return Ok(new { isOn = MachineStateService.IsMachineOn });
        }
    }
}
