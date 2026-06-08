using DeunaCall.Data;
using DeunaCall.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[Authorize(Roles = "Nurse,SuperAdmin")]
[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PatientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("admit")]
    public async Task<IActionResult> AdmitPatient([FromBody] Patient model)
    {
        var code = new Random().Next(100000, 999999).ToString();
        model.AccessCode = code;

        _context.Patients.Add(model);
        await _context.SaveChangesAsync();

        return Ok(model);
    }
}
