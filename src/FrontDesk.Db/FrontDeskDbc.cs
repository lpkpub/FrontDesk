using FrontDesk.Db.Config;
using FrontDesk.Db.Data;
using FrontDesk.Db.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FrontDesk.Db;

public sealed class FrontDeskDbc : DbContext
{
    /// <summary>
    /// Required for generating EFCore migrations.
    /// </summary>
    public FrontDeskDbc() { }

    /// <summary>
    /// For DI.
    /// </summary>
    public FrontDeskDbc(DbContextOptions<FrontDeskDbc> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .UseNpgsql()
                .UseSnakeCaseNamingConvention()
                .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .ConfigureConverters()
            .PreventCascadingDelete()
            .AddSeedData();
    }

    public DbSet<User> User { get; set; }
    public DbSet<Visit> Visit { get; set; }
}
