namespace NjianiAPI.Models;

public class ClaimT
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public string HashValue { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
    public ClaimSeverity? Severity { get; set; }
    public string? Comment { get; set; } = string.Empty;

    public User? User { get; set; }
    public Guid UserId { get; set; }

    public Location? Location { get; set; }
    public Guid LocationId { get; set; }

    public List<ClaimImage> Images { get; set; } = new List<ClaimImage>();
}