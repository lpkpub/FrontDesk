using FrontDesk.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace FrontDesk.Shared.Dto;

public sealed class VisitRequest
{
    public UserType UserType { get; init; }
    public VisitAction VisitAction { get; init; }

    [StringLength(6, MinimumLength = 6)]
    [RegularExpression(@"^\d{6}$")] // Numbers only.
    public string Pin { get; init; } = "000000";
    public string Name { get; init; } = string.Empty;
    public string? Company { get; init; }
    public string? PhoneNumber { get; init; }
    public string Reason { get; init; } = string.Empty;
}
