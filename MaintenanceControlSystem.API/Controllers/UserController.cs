using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MaintenanceControlSystem.Domain.Entities;
using MaintenanceControlSystem.Domain.Enums;
using MaintenanceControlSystem.Application.DTOs;
using MaintenanceControlSystem.Infrastructure.Repositories.Interfaces;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceControlSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public UserController(IUserRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        var existingUser = await _userRepository.GetByEmailAsync(user.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = $"Já existe um usuário com o e-mail {user.Email}." });
        }

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.PasswordHash));
        user.PasswordHash = Convert.ToBase64String(hash);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProfile), new { id = user.Id }, new
        {
            message = "Usuário registrado com sucesso.",
            data = new { user.Id, user.Email, user.Role }
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {


        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Credenciais inválidas." });
        }

        Console.WriteLine($"EMAIL: {request.Email}");

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
        var incomingPasswordHash = Convert.ToBase64String(hash);

        Console.WriteLine($"Senha digitada (hash base64): {incomingPasswordHash}");
        Console.WriteLine($"Senha no banco: {user.PasswordHash}");

        if (user.PasswordHash != incomingPasswordHash)
        {
            return Unauthorized(new { message = "Credenciais inválidas." });
        }

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            message = "Login realizado com sucesso.",
            data = new LoginResponse
            {
                Token = token,
                Name = user.Name,
                Role = user.Role.ToString()
            }
        });

    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["JwtSettings:ExpireMinutes"]!));

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity == null)
            return Unauthorized(new { message = "Usuário não autenticado." });

        var claims = identity.Claims;

        var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        return Ok(new
        {
            message = "Perfil carregado com sucesso.",
            data = new
            {
                UserId = userId,
                Name = name,
                Role = role
            }
        });
    }

    [HttpGet("list")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();

        var response = users.Select(u => new UserListResponse
        {
            Id = u.Id,
            Email = u.Email,
            Role = u.Role.ToString()
        });

        return Ok(new
        {
            message = "Lista de usuários carregada com sucesso.",
            data = response
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest dto)
    {
        var existing = await _userRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = $"Usuário com ID {id} não foi encontrado." });

        existing.Username = dto.Username;
        existing.Role = dto.Role;

        _userRepository.Update(existing);
        await _userRepository.SaveChangesAsync();

        return Ok(new { message = "Usuário atualizado com sucesso." });
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = $"Usuário com ID {id} não foi encontrado." });
        }

        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync();

        return Ok(new { message = "Usuário deletado com sucesso." });
    }
}
