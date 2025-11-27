using Microsoft.EntityFrameworkCore;
using NjianiAPI.Data;
using NjianiAPI.Models;

public class AnalyticsService : IAnalyticsService
{
    private readonly NjianiDbContext _context;

    public AnalyticsService(NjianiDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardAnalyticsResponseDto> GetDashboardAnalyticsAsync()
    {
        var now = DateTime.UtcNow;
        var today = DateTime.SpecifyKind(now.Date, DateTimeKind.Utc);
        var startOfMonth = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1), DateTimeKind.Utc);
        var startOfLastMonth = startOfMonth.AddMonths(-1);
        var endOfLastMonth = startOfMonth.AddDays(-1);

        // Get current month claims
        var currentMonthClaims = await _context.Claims
            .Where(c => c.CreatedAt >= startOfMonth)
            .ToListAsync();

        // Get last month claims
        var lastMonthClaims = await _context.Claims
            .Where(c => c.CreatedAt >= startOfLastMonth && c.CreatedAt <= endOfLastMonth)
            .ToListAsync();

        // Get today's claims
        var todayClaims = await _context.Claims
            .Where(c => c.CreatedAt >= today)
            .ToListAsync();

        // Calculate statistics
        var analytics = new DashboardAnalyticsResponseDto
        {
            TotalClaims = CalculateStatistics(
                currentMonthClaims.Count,
                lastMonthClaims.Count
            ),
            VerifiedClaims = CalculateStatistics(
                currentMonthClaims.Count(c => c.Status == ClaimStatus.Resolved),
                lastMonthClaims.Count(c => c.Status == ClaimStatus.Resolved)
            ),
            PendingClaims = CalculateStatistics(
                currentMonthClaims.Count(c => c.Status == ClaimStatus.Pending),
                lastMonthClaims.Count(c => c.Status == ClaimStatus.Pending)
            ),
            RejectedClaims = CalculateStatistics(
                currentMonthClaims.Count(c => c.Status == ClaimStatus.Rejected),
                lastMonthClaims.Count(c => c.Status == ClaimStatus.Rejected)
            ),
            RecentClaims = await GetRecentClaimsAsync(),
            TodayActivity = new TodayActivityDto
            {
                NewClaims = todayClaims.Count,
                Processed = todayClaims.Count(c => c.Status == ClaimStatus.InProgress),
                Verified = todayClaims.Count(c => c.Status == ClaimStatus.Resolved),
                Rejected = todayClaims.Count(c => c.Status == ClaimStatus.Rejected)
            }
        };

        return analytics;
    }
    
    private ClaimStatistics CalculateStatistics(int currentCount, int lastMonthCount)
    {
        if (lastMonthCount == 0)
        {
            return new ClaimStatistics
            {
                Count = currentCount,
                ChangePercentage = currentCount > 0 ? "+100%" : "0%",
                ChangeType = currentCount > 0 ? ChangeType.Increase : ChangeType.NoChange
            };
        }

        var percentageChange = ((double)(currentCount - lastMonthCount) / lastMonthCount) * 100;
        var changeType = percentageChange > 0 ? ChangeType.Increase :
                        percentageChange < 0 ? ChangeType.Decrease :
                        ChangeType.NoChange;

        var sign = percentageChange > 0 ? "+" : "";
        var changePercentageStr = $"{sign}{percentageChange:F1}%";

        return new ClaimStatistics
        {
            Count = currentCount,
            ChangePercentage = changePercentageStr,
            ChangeType = changeType
        };
    }
    private async Task<List<RecentClaimDto>> GetRecentClaimsAsync()
    {
        var recentClaims = await _context.Claims
            .Include(c => c.Location)
            .OrderByDescending(c => c.CreatedAt)
            .Take(5)
            .Select(c => new
            {
                c.Id,
                LocationName = c.Location != null ? c.Location.Name : "Unknown Location",
                c.Status,
                c.CreatedAt
            })
            .ToListAsync();
        
        return recentClaims.Select(c => new RecentClaimDto
        {
            ClaimId = c.Id.ToString(),
            Location = c.LocationName,
            Status = c.Status,
            TimeAgo = GetTimeAgo(c.CreatedAt)
        }).ToList();
    }
    private string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalSeconds < 60)
            return $"{(int)timeSpan.TotalSeconds} sec ago";

        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} min ago";

        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} hr ago";

        if (timeSpan.TotalDays < 30)
            return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays != 1 ? "s" : "")} ago";

        var months = (int)(timeSpan.TotalDays / 30);
        return $"{months} month{(months != 1 ? "s" : "")} ago";
    }

    public async Task<AnalyticsDashboardResponseDto> GetAnalyticsDashboardAsync()
    {
        var now = DateTime.UtcNow;
        var startOfMonth = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1), DateTimeKind.Utc);
        var startOfLastMonth = startOfMonth.AddMonths(-1);
        var endOfLastMonth = startOfMonth.AddDays(-1);
        var startOfWeek = DateTime.SpecifyKind(now.Date.AddDays(-(int)now.DayOfWeek), DateTimeKind.Utc);

        // Get current month claims
        var currentMonthClaims = await _context.Claims
            .Include(c => c.Location)
            .Where(c => c.CreatedAt >= startOfMonth)
            .ToListAsync();

        // Get last month claims
        var lastMonthClaims = await _context.Claims
            .Where(c => c.CreatedAt >= startOfLastMonth && c.CreatedAt <= endOfLastMonth)
            .ToListAsync();

        // Get this week's claims
        var thisWeekClaims = await _context.Claims
            .Where(c => c.CreatedAt >= startOfWeek)
            .ToListAsync();

        var analytics = new AnalyticsDashboardResponseDto
        {
            TotalClaims = CalculateStatistics(
                currentMonthClaims.Count,
                lastMonthClaims.Count
            ),
            VerificationRate = await CalculateVerificationRateAsync(startOfMonth, startOfLastMonth, endOfLastMonth),
            ActiveUsersClaims = await CalculateActiveUsersClaimsAsync(startOfMonth, startOfLastMonth, endOfLastMonth),
            AvgProcessingTime = await CalculateAvgProcessingTimeAsync(startOfMonth, startOfLastMonth, endOfLastMonth),
            ClaimsOverTime = await GetClaimsOverTimeAsync(),
            ClaimsBySeverity = await GetClaimsBySeverityAsync(),
            TopClaimLocations = await GetTopClaimLocationsAsync(),
            ProcessingSpeed = await GetProcessingSpeedAsync(),
            ThisWeek = new WeekActivityDto
            {
                NewClaims = thisWeekClaims.Count,
                Processed = thisWeekClaims.Count(c => c.Status == ClaimStatus.InProgress),
                Verified = thisWeekClaims.Count(c => c.Status == ClaimStatus.Resolved),
                Rejected = thisWeekClaims.Count(c => c.Status == ClaimStatus.Rejected),
                Backlog = await _context.Claims.CountAsync(c => c.Status == ClaimStatus.Pending)
            }
        };

        return analytics;
    }

    private async Task<ClaimStatistics> CalculateVerificationRateAsync(DateTime startOfMonth, DateTime startofLastMonth, DateTime endOfLastMonth)
    {
        var currentMonthTotal = await _context.Claims
            .CountAsync(c => c.CreatedAt >= startOfMonth);
        
        var currentMonthVerified = await _context.Claims
            .CountAsync(c => c.CreatedAt >= startOfMonth && c.Status == ClaimStatus.Resolved);

        var lastMonthTotal = await _context.Claims
            .CountAsync(c => c.CreatedAt >= startofLastMonth && c.CreatedAt <= endOfLastMonth);
        
        var lastMonthVerified = await _context.Claims
            .CountAsync(c => c.CreatedAt >= startofLastMonth && c.CreatedAt <= endOfLastMonth 
            && c.Status == ClaimStatus.Resolved);

        var currentRate = currentMonthTotal > 0 ? (double) currentMonthVerified / currentMonthTotal * 100 : 0;
        var lastRate = lastMonthTotal > 0 ? (double) lastMonthVerified / lastMonthTotal * 100 : 0;

        return new ClaimStatistics
        {
            Count = (int)Math.Round(currentRate),
            ChangePercentage = CalculatePercentageChange(currentRate, lastRate),
            ChangeType = DetermineChangeType(currentRate, lastRate)
        };
    }
    
    private async Task<ClaimStatistics> CalculateActiveUsersClaimsAsync(DateTime startOfMonth, DateTime startofLastMonth, DateTime endOfLastMonth)
    {
        var currentMonthActiveUsers = await _context.Claims
            .Where(c => c.CreatedAt >= startOfMonth)
            .Select(c => c.UserId)
            .Distinct()
            .CountAsync();
        var lastMonthActiveUsers = await _context.Claims
            .Where(c => c.CreatedAt >= startofLastMonth && c.CreatedAt <= endOfLastMonth)
            .Select(c => c.UserId)
            .Distinct()
            .CountAsync();

        return new ClaimStatistics
        {
            Count = currentMonthActiveUsers,
            ChangePercentage = CalculatePercentageChange(currentMonthActiveUsers, lastMonthActiveUsers),
            ChangeType = DetermineChangeType(currentMonthActiveUsers, lastMonthActiveUsers)
        };
    }
    
    private async Task<ProcessingTimeStats> CalculateAvgProcessingTimeAsync(DateTime startOfMonth, DateTime startofLastMonth, DateTime endOfLastMonth)
    {
        var currentMonthProcessed = await _context.Claims
            .Where(c => c.CreatedAt >= startOfMonth && c.Status == ClaimStatus.Resolved)
            .ToListAsync();
        var lastMonthProcessed = await _context.Claims
            .Where(c => c.CreatedAt >= startofLastMonth && c.CreatedAt <= endOfLastMonth && c.Status == ClaimStatus.Resolved)
            .ToListAsync();
        
        var currentAvg = currentMonthProcessed.Count > 0 ? 
        currentMonthProcessed.Average(c => (c.UpdatedAt - c.CreatedAt).TotalHours) 
        : 0;
        var lastAvg = lastMonthProcessed.Count > 0 ? 
        lastMonthProcessed.Average(c => (c.UpdatedAt - c.CreatedAt).TotalHours) 
        : 0;

        return new ProcessingTimeStats
        {
            AvgHrs = Math.Round(currentAvg, 2),
            ChangePercentage = CalculatePercentageChange(currentAvg, lastAvg),
            ChangeType = DetermineChangeType(lastAvg, currentAvg)
        };
    }
    
    private async Task<List<MonthlyClaimDto>> GetClaimsOverTimeAsync()
{
    var currentYear = DateTime.UtcNow.Year;

    var monthlyData = await _context.Claims
        .Where(c => c.CreatedAt.Year == currentYear)
        .GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
        .Select(g => new MonthlyClaimDto
        {
            Year = g.Key.Year,
            Month = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
            Count = g.Count()
        })
        .OrderBy(m => m.Year)
        .ThenBy(m => DateTime.ParseExact(m.Month, "MMMM", System.Globalization.CultureInfo.CurrentCulture).Month) //check later
        .ToListAsync();

    return monthlyData;
}
    
    private string CalculatePercentageChange(double current, double previous)
    {
        if (previous == 0)
        {
            return current > 0 ? "+100%" : "0%";
        }

        var percentageChange = ((current - previous) / previous) * 100;
        var sign = percentageChange > 0 ? "+" : "";
        return $"{sign}{percentageChange:F1}%";
    }

    private ChangeType DetermineChangeType(double current, double previous)
    {
        if (current > previous) return ChangeType.Increase;
        if (current < previous) return ChangeType.Decrease;
        return ChangeType.NoChange;
    }
      
}