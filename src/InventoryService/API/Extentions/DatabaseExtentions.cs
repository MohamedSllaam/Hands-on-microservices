using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace API.Extentions;

public static class DatabaseExtentions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {

        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<InventoryDbContext>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }

    }
}