using FrontDesk.Db.Tables;
using FrontDesk.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace FrontDesk.Db.Data;

public static class SeedData
{
    // User Guid
    static readonly Guid U01 = Guid.Parse("b2d8f9a4-1a3e-4e6f-8c02-000000000001");
    static readonly Guid U02 = Guid.Parse("b2d8f9a4-1a3e-4e6f-8c02-000000000002");
    static readonly Guid U03 = Guid.Parse("b2d8f9a4-1a3e-4e6f-8c02-000000000003");

    public static ModelBuilder AddSeedData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User { Id = U01, Name = "Office Worker", Pin = "111111", UserType = UserType.Staff, IsActive = true },
            new User { Id = U02, Name = "Warehouse Worker", Pin = "222222", UserType = UserType.Staff, IsActive = true },
            new User { Id = U03, Name = "Casual Worker", Pin = "333333", UserType = UserType.Staff, IsActive = false });

        return modelBuilder;
    }
}
