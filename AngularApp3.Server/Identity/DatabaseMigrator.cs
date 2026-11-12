using AngularApp3.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace AngularApp3.Server.Identity;

public static class DatabaseMigrator
{
    public static async Task MigrateAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
