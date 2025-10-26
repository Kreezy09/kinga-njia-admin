public class ClaimImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public Claim? Claim { get; set; }
    public Guid ClaimId { get; set; }
}