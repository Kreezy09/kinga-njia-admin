public interface IAnalyticsService
{
    Task<DashboardAnalyticsResponseDto> GetDashboardAnalyticsAsync();
    Task<AnalyticsDashboardResponseDto> GetAnalyticsDashboardAsync();
}