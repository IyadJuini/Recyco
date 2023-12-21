using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectRecycle.Utility;

namespace ProjectRecycle.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MyContext(
                      serviceProvider.GetRequiredService<
                        DbContextOptions<MyContext>>()))
            {
                if (context.AppUsers.Any())
                {
                    return;   // DB has been seeded
                }

                AppUser user = new AppUser();
                user.FirstName = "Super";
                user.LastName = "Admin";
                user.Email = "admin@admin.com";
                user.Role = StaticData.UserRole.Admin;
                user.Diploma = "null";
                user.Expertise = "null";


                PasswordHasher<AppUser> Hasher = new PasswordHasher<AppUser>();
                user.Password = Hasher.HashPassword(user, "123123123");

                context.AppUsers.Add(user);
                context.SaveChanges();

            }
        }
    }
}
