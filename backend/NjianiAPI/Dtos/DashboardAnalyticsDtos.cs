public class DashboardAnalyticsResponseDto
{
    public ClaimStatistics TotalClaims { get; set; } = new();
    public ClaimStatistics VerifiedClaims { get; set; } = new();
    public ClaimStatistics PendingClaims { get; set; } = new();
    public ClaimStatistics RejectedClaims { get; set; } = new();
    public List<RecentClaimDto> RecentClaims { get; set; } = new();
    public TodayActivityDto TodayActivity { get; set; } = new();
}

public class ClaimStatistics
{
    public int Count { get; set; }
    public string ChangePercentage { get; set; } = string.Empty;
    public ChangeType ChangeType { get; set; }
}

public enum ChangeType
{
    Increase,
    Decrease,
    NoChange
}

public class RecentClaimDto
{
    public string ClaimId { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public ClaimStatus Status { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
}

public class TodayActivityDto
{
    public int NewClaims { get; set; }
    public int Processed { get; set; }
    public int Verified { get; set; }
    public int Rejected { get; set; }
}