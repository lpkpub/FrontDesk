using FrontDesk.Db.Tables;
using Microsoft.EntityFrameworkCore;

namespace FrontDesk.Db.Config;

public static class CustomConfig
{
    public static ModelBuilder ConfigureConverters(this ModelBuilder modelBuilder)
    {
        // Keep UserType enums readable in the database.
        modelBuilder.Entity<User>()
            .Property(u => u.UserType)
            .HasConversion<string>()
            .HasMaxLength(20);

        return modelBuilder;
    }

    public static ModelBuilder PreventCascadingDelete(this ModelBuilder modelBuilder)
    {
        var foreignKeys = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys());
        foreach (var foreignKey in foreignKeys)
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        return modelBuilder;
    }
}
