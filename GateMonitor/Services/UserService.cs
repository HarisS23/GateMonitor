using GateMonitor.Data;
using GateMonitor.Models;
using GateMonitor.Models.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GateMonitor.Services
{
    public class UserService
    {
        private readonly AppDbContext _db;
        public UserService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<LoginResult> ValidateUser(string username, string password)
        {
            var hasher = new PasswordHasher<User>();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return new LoginResult(false, "Invalid username or password");
            }

            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

            return result == PasswordVerificationResult.Failed ? 
                new LoginResult(false, "Invalid username or password") : 
                new LoginResult(true, "Login successful");
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
