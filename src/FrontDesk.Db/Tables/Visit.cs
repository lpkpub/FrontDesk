namespace FrontDesk.Db.Tables;

public class Visit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime EntryAt { get; set; }
    public DateTime? ExitAt { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string? Company { get; set; }
    public string? PhoneNumber { get; set; }
    public required string Reason { get; set; }
}
