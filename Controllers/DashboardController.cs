using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Server.DTOs;
using Server.Services.Interfaces;
using System.Security.Claims;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var response = await _dashboardService.GetSummaryAsync(userId);
            return Ok(response);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentTransactions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var response = await _dashboardService.GetRecentTransactionsAsync(userId);
            return Ok(response);
        }

        [HttpGet("reports/category-summary")]
        public async Task<IActionResult> GetCategorySummary()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var response = await _dashboardService.GetCategorySummaryAsync(userId);
            return Ok(response);
        }

        [HttpGet("reports/monthly-summary")]
        public async Task<IActionResult> GetMonthlySummary()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var response = await _dashboardService.GetMonthlySummaryAsync(userId);
            return Ok(response);
        }
    }
} 