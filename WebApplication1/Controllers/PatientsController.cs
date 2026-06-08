using DeunaCall.Data;
using DeunaCall.Shared.DTOs;
using DeunaCall.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var patients = await _context.Patients.ToListAsync();
        return Ok(patients);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null) return NotFound();
        return Ok(patient);
    }

    [HttpPost("admit")]
    public async Task<IActionResult> AdmitPatient([FromBody] CreatePatientDto model)
    {
        var code = new Random().Next(100000, 999999).ToString();
        var patient = new Patient
        {
            Name = model.Name,
            RoomNumber = model.RoomNumber,
            AccessCode = code
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return Ok(patient);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePatientDto model)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null) return NotFound();

        patient.Name = model.Name;
        patient.RoomNumber = model.RoomNumber;
        await _context.SaveChangesAsync();

        return Ok(patient);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null) return NotFound();

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
