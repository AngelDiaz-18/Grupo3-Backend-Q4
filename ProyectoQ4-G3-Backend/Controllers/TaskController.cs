using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoQ4_G3_Backend.DTOs;
using ProyectoQ4_G3_Backend.Models;
using ProyectoQ4_G3_Backend.Services;

namespace ProyectoQ4_G3_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly TaskServices _taskServices;
    
    public  TaskController(TaskServices taskServices)
    {
        _taskServices = taskServices;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (String.IsNullOrEmpty(userId))
                return Unauthorized();
            
            var task = await _taskServices.Create(dto, userId);
            return Ok(task);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMyTasks()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (String.IsNullOrEmpty(userId))
                return Unauthorized();
            
            var task = await _taskServices.GetByUser(userId);
            
            return Ok(task);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }
    
}