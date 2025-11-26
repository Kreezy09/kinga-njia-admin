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
        var today = now.Date;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
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
}