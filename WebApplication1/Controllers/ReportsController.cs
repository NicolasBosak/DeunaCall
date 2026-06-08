using DeunaCall.Data;
using DeunaCall.Shared.DTOs;
using DeunaCall.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers;

[Authorize(Roles = "SuperAdmin")]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlyReport([FromQuery] int year, [FromQuery] int month)
    {
        var requests = await _context.CallRequests
            .Include(r => r.Patient)
            .Where(r => r.CreatedAt.Year == year && r.CreatedAt.Month == month)
            .ToListAsync();

        var attended = requests.Where(r => r.ResolvedAt.HasValue).ToList();
        var responseTimes = attended
            .Select(r => (r.ResolvedAt!.Value - r.CreatedAt).TotalMinutes)
            .ToList();

        var report = new MonthlyReportDto
        {
            TotalRequests = requests.Count,
            ByType = requests
                .GroupBy(r => r.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            ByStatus = requests
                .GroupBy(r => r.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            AverageResponseTimeMinutes = responseTimes.Any() ? Math.Round(responseTimes.Average(), 1) : 0,
            FastestResponseMinutes = responseTimes.Any() ? Math.Round(responseTimes.Min(), 1) : 0,
            SlowestResponseMinutes = responseTimes.Any() ? Math.Round(responseTimes.Max(), 1) : 0,
            RequestsByDay = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                .Select(day => new DailyCountDto
                {
                    Day = day,
                    Count = requests.Count(r => r.CreatedAt.Day == day)
                }).ToList()
        };

        return Ok(report);
    }
}
