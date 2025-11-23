using System.Security.Claims;
using GateMonitor.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GateMonitor.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserService _userService; // your DB logic

        public LoginModel(UserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> OnPostAsync(string username, string password)
        {
            var loginResult = await _userService.ValidateUser(username, password);

            if (!loginResult.Success)
            {
                ModelState.AddModelError("", "Invalid login.");
                return Page();
            }

            var user = await _userService.GetUserByUsername(username);

            if(user == null)
            {
                ModelState.AddModelError("", "Invalid login.");
                return Page();
            }
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);
            
            await HttpContext.SignInAsync("MyCookieAuth", principal);

            return RedirectToPage("/Index");
        }
    }
}
