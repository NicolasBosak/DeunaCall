using DeunaCall.Data.Models;
using DeunaCall.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[Authorize(Roles = "SuperAdmin")]
[ApiController]
[Route("api/[controller]")]
public class NursesController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public NursesController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var nurses = await _userManager.GetUsersInRoleAsync("Nurse");
        var result = nurses.Select(n => new NurseListItemDto
        {
            Id = n.Id,
            FullName = n.FullName,
            UserName = n.UserName ?? "",
            Email = n.Email ?? ""
        }).ToList();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNurseDto model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email,
            FullName = model.FullName
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        await _userManager.AddToRoleAsync(user, "Nurse");

        return Ok(new NurseListItemDto
        {
            Id = user.Id,
            FullName = user.FullName,
            UserName = user.UserName ?? "",
            Email = user.Email ?? ""
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        await _userManager.DeleteAsync(user);
        return NoContent();
    }
}
