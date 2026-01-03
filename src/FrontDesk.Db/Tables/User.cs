using FrontDesk.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FrontDesk.Db.Tables;

[Index(nameof(Pin))]
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }

    [StringLength(6, MinimumLength = 6)]
    [RegularExpression(@"^\d{6}$")] // Numbers only.
    public string? Pin { get; set; }
    public UserType UserType { get; set; }
    public bool IsActive { get; set; }
    public ICollection<Visit> Visits { get; } = [];
}
