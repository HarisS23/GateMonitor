using GateMonitor.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GateMonitor.Controllers
{
    [ApiController]
    [Route("api/workers")]
    public class WorkersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public WorkersController(AppDbContext db) => _db = db;

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var worker = await _db.Workers.FindAsync(id);
            if (worker == null) return NotFound();

            _db.Workers.Remove(worker);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPatch("access/{id}/{hasAccess}")]
        public async Task<IActionResult> ChangeWorkerAccess(int id, bool hasAccess)
        {
            var worker = await _db.Workers.FindAsync(id);
            if (worker == null) return NotFound();
            worker.HasAccess = hasAccess;
            _db.Workers.Update(worker);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
