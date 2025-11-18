public class ClaimCreateDto
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ClaimSeverity? Severity { get; set; }
    public string? Comment { get; set; } = string.Empty;
    public List<IFormFile>? Images { get; set; }
    public Guid LocationId { get; set; }
    // public Guid? UserId { get; set; }
}

public class ClaimImageCreateDto
{
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
}

public class ClaimUpdateDto
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public Guid UserId { get; set; }
}
public class ClaimResponseDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string HashValue { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ClaimStatus Status { get; set; }
    public ClaimSeverity? Severity { get; set; }
    public string Comment { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public Guid UserId { get; set; }
    public List<ClaimImageResponseDto> Images { get; set; } = new();
}

public class ClaimImageResponseDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public DateTime UploadedAt { get; set; }
}