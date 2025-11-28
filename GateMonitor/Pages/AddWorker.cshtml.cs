using GateMonitor.Data;
using GateMonitor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GateMonitor.Pages
{
    [Authorize]
    public class AddWorkerModel : PageModel
    {
        private readonly AppDbContext _dbContext;

        [BindProperty]
        public Worker NewWorker { get; set; }
        
        public AddWorkerModel(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            NewWorker.HasAccess = true;
            NewWorker.CreatedAt = DateTime.UtcNow;

            var rfidCard = await _dbContext.RfidCards.FirstOrDefaultAsync(rc => rc.Uid == NewWorker.RfidUid);

            if (rfidCard == null)
            {
                _dbContext.RfidCards.Add(new RfidCard
                {
                    Uid = NewWorker.RfidUid,
                    CreatedAt = DateTime.UtcNow
                });
            }

            _dbContext.Workers.Add(NewWorker);
            await _dbContext.SaveChangesAsync();

            return RedirectToPage("Workers");
        }
    }
}
