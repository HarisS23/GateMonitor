using GateMonitor.Data;
using GateMonitor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GateMonitor.Pages
{
    [Authorize]
    public class WorkersModel : PageModel
    {
        private readonly AppDbContext _dbContext;
        
        public WorkersModel(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public List<Worker> Workers { get; set; } = new List<Worker>();

        public async Task OnGet()
        {
            Workers = await _dbContext.Workers.ToListAsync();
        }

        public IActionResult OnPost()
        {
            return RedirectToPage();
        }
    }
}
