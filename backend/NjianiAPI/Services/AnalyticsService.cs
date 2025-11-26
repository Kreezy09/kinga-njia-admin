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
}