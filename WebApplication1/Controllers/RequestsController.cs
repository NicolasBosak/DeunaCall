using DeunaCall.Data;
using DeunaCall.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Hubs;

namespace WebApplication1.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<CallHub> _hubContext;

    public RequestsController(ApplicationDbContext context, IHubContext<CallHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [Authorize(Roles = "Patient")]
    [HttpPost]
    public async Task<IActionResult> CreateRequest([FromBody] CallRequest request)
    {
        request.CreatedAt = DateTime.UtcNow;
        request.Status = RequestStatus.Pending;

        _context.CallRequests.Add(request);
        await _context.SaveChangesAsync();

        var requestWithPatient = await _context.CallRequests.Include(r => r.Patient).FirstOrDefaultAsync(r => r.Id == request.Id);

        await _hubContext.Clients.All.SendAsync("ReceiveNewCall", requestWithPatient);

        return Ok(requestWithPatient);
    }

    [Authorize(Roles = "Nurse,SuperAdmin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] RequestStatus newStatus)
    {
        var request = await _context.CallRequests.FindAsync(id);
        if (request == null) return NotFound();

        request.Status = newStatus;
        if (newStatus == RequestStatus.Attended)
        {
            request.ResolvedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("CallStatusChanged", request.Id, newStatus);

        return Ok(request);
    }
    
    [Authorize(Roles = "Nurse,SuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetActiveRequests()
    {
        var requests = await _context.CallRequests
            .Include(r => r.Patient)
            .Where(r => r.Status != RequestStatus.Attended)
            .OrderByDescending(r => r.Type == RequestType.Emergency)
            .ThenBy(r => r.CreatedAt)
            .ToListAsync();
            
        return Ok(requests);
    }
}
