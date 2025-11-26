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
}