using GateMonitor.Models;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GateMonitor.Data
{
    public static class AppDbSeeder
    {
        public static async Task Seed(AppDbContext db)
        {
            
            if(!await db.Users.AnyAsync())
            {
                var hasher = new PasswordHasher<User>();
                await db.Users.AddAsync(
                    new User
                    {
                        Username = "admin",
                        PasswordHash = hasher.HashPassword(null, "admin123"),
                        CreatedAt = DateTime.UtcNow
                    }
                );

                await db.SaveChangesAsync();
            }

            if (!await db.RfidCards.AnyAsync())
            {
                await db.RfidCards.AddAsync(
                new RfidCard
                {
                    Uid = "admin",
                    CreatedAt = DateTime.UtcNow
                });

                await db.SaveChangesAsync();
            }

            if (!await db.RfidScanActions.AnyAsync())
            {
                db.RfidScanActions.AddRange(
                    new RfidScanAction
                    {
                        Name = "TURNED_ON",
                        PrettyName = "Turned On",
                    },
                    new RfidScanAction
                    {
                        Name = "TURNED_OFF",
                        PrettyName = "Turned Off",
                    }
                );
            }

            await db.SaveChangesAsync();
        }
    }

}
