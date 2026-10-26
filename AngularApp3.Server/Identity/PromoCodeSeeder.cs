using AngularApp3.Server.Data;
using AngularApp3.Server.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AngularApp3.Server.Identity;

public static class PromoCodeSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await dbContext.PromoCodes.AnyAsync(p => p.Code == "SAVE10"))
        {
            return;
        }

        dbContext.PromoCodes.Add(new PromoCode
        {
            Code = "SAVE10",
            DiscountPercentage = 10,
            IsActive = true
        });
        await dbContext.SaveChangesAsync();
    }
}
