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
}