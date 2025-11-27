using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NjianiAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    /// <summary>
    /// Gets comprehensive dashboard analytics including claim statistics and activity
    /// </summary>
    /// <returns>Dashboard analytics data</returns>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardAnalytics()
    {
        try
        {
            var analytics = await _analyticsService.GetDashboardAnalyticsAsync();
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving dashboard analytics: {ex.Message}");
            return StatusCode(500, new { message = "An error occurred while retrieving analytics data" });
        }
    }

    /// <summary>
    /// Gets detailed analytics for the analytics tab including trends, processing times, and distributions
    /// </summary>
    /// <returns>Detailed analytics data</returns>
    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailedAnalytics()
    {
        try
        {
            var analytics = await _analyticsService.GetAnalyticsDashboardAsync();
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving detailed analytics: {ex.Message}");
            return StatusCode(500, new { message = "An error occurred while retrieving detailed analytics data" });
        }
    }
}