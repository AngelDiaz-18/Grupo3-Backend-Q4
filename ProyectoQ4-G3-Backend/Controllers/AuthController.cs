using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoQ4_G3_Backend.DTOs;
using ProyectoQ4_G3_Backend.Services;

namespace ProyectoQ4_G3_Backend.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthServices _authServices;

    public AuthController(AuthServices authServices)
    {
        _authServices = authServices;
    }
    
    //POST /api/Auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistroDto dto)
    {
        try
        {
            var user = await _authServices.Register(dto);
            return Ok(new { user.Id, user.FullName, user.Email });
        }
        catch (Exception ex)
        {
            return BadRequest(new {message = ex.Message});
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var token = await _authServices.Login(dto);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return BadRequest(new {message = ex.Message});
        }
    }
}