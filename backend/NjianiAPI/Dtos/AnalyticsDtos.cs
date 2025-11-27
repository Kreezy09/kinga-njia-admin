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

public class AnalyticsDashboardResponseDto
{
    public ClaimStatistics TotalClaims { get; set; } = new();
    public ClaimStatistics VerificationRate { get; set; } = new();
    public ClaimStatistics ActiveUsersClaims { get; set; } = new();
    public ProcessingTimeStats AvgProcessingTime { get; set; } = new();
    public List<MonthlyClaimDto> ClaimsOverTime { get; set; } = new();
    public List<SeverityDistributionDto> ClaimsBySeverity { get; set; } = new();
    public List<LocationClaimDto> TopClaimLocations { get; set; } = new();
    public ProcessingSpeedDto ProcessingSpeed { get; set; } = new();
    public WeekActivityDto ThisWeek { get; set; } = new();
}

public class ProcessingTimeStats
{
    public double AvgHrs { get; set; }
    public string ChangePercentage { get; set; } = string.Empty;
    public ChangeType ChangeType { get; set; }
}

public class MonthlyClaimDto
{
    
    public string Month { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Count { get; set; }
}
public class SeverityDistributionDto
{
    public string Severity { get; set; } = string.Empty;
    public int Count { get; set; }
}
public class LocationClaimDto
{
    public string Location { get; set; } = string.Empty;
    public int ClaimCount { get; set; }
}

public class ProcessingSpeedDto{
    public double AvgTimeHrs { get; set; }
    public double FastestTimeHrs { get; set; }
    public double SlowestTimeHrs { get; set; }
}

public class WeekActivityDto{
    public int NewClaims { get; set; }
    public int Processed { get; set; }
    public int Verified { get; set; }
    public int Rejected { get; set; }
    public int Backlog { get; set; }
}