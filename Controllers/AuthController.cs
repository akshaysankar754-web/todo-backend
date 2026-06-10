using Microsoft.AspNetCore.Mvc;
using todo_backend.Data;
using todo_backend.DTOs;
using todo_backend.Models;
using todo_backend.Services;

namespace todo_backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(
        AppDbContext context,
        JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterDto dto)
    {
        if (_context.Users.Any(x => x.Email == dto.Email))
        {
            return BadRequest("Email already exists");
        }

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok("User registered");
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        var user = _context.Users
            .FirstOrDefault(x => x.Email == dto.Email);

        if (user == null)
        {
            return Unauthorized("Invalid email");
        }

        bool validPassword =
            BCrypt.Net.BCrypt.Verify(
                dto.Password,
                user.PasswordHash);

        if (!validPassword)
        {
            return Unauthorized("Invalid password");
        }

        var token =
            _jwtService.GenerateToken(user);

        return Ok(new { token });
    }
}