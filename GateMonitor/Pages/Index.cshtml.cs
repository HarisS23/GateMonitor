using GateMonitor.Data;
using GateMonitor.Models;
using GateMonitor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GateMonitor.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext _dbContext;

        public IndexModel(ILogger<IndexModel> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public bool IsMachineOn { get; private set; }

        [BindProperty] 
        public List<RfidScanRecord> RfidRecords { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        private const int PageSize = 20;

        public async Task OnGet(int pageNumber = 1)
        {
            IsMachineOn = MachineStateService.IsMachineOn; 

            PageNumber = pageNumber;

            var totalRecords = await _dbContext.RfidScanRecords.CountAsync();
            TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            RfidRecords = await _dbContext.RfidScanRecords
                .Include(r => r.Worker)
                .Include(r => r.RfidScanAction)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        public static string FormatTimestamp(DateTime dt)
        {
            var now = DateTime.Now;

            bool isToday = dt.Date == now.Date;

            if (isToday)
            {
                return dt.ToString("HH:mm:ss");
            }
            else
            {
                return dt.ToString("dd-MM-yyyy HH:mm");
            }
        }

    }
}
