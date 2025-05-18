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
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

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
        Console.WriteLine("=== REGISTRANDO USUÁRIO ===");
        Console.WriteLine($"Nome: {user.Name}");
        Console.WriteLine($"Email: {user.Email}");
        Console.WriteLine($"Senha (Texto Puro): {user.PasswordHash}");

        var existingUser = await _userRepository.GetByEmailAsync(user.Email);
        if (existingUser != null)
        {
            Console.WriteLine("ERRO: Usuário já existe.");
            return BadRequest(new { message = $"Já existe um usuário com o e-mail {user.Email}." });
        }

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(user.PasswordHash));
        user.PasswordHash = Convert.ToBase64String(hash);

        Console.WriteLine($"Senha Hasheada (Base64): {user.PasswordHash}");

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        Console.WriteLine("=== USUÁRIO REGISTRADO COM SUCESSO ===");
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
        Console.WriteLine("=== TENTANDO LOGIN ===");
        Console.WriteLine($"Email: {request.Email}");
        Console.WriteLine($"Senha Digitada: {request.Password}");

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            Console.WriteLine("ERRO: Usuário não encontrado.");
            return Unauthorized(new { message = "Credenciais inválidas." });
        }

        Console.WriteLine($"Usuário encontrado: {user.Email}");
        Console.WriteLine($"Senha Armazenada (Hash): {user.PasswordHash}");

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
        var hashHex = BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();

        Console.WriteLine($"Senha Digitada (Hash Hex): {hashHex}");

        if (user.PasswordHash != hashHex)
        {
            Console.WriteLine("ERRO: Senha Incorreta.");
            return Unauthorized(new { message = "Credenciais inválidas." });
        }

        Console.WriteLine("=== LOGIN BEM SUCEDIDO ===");
        var token = GenerateJwtToken(user);

        return Ok(new
        {
            message = "Login realizado com sucesso.",
            data = new LoginResponse
            {
                Token = token,
                Name = user.Name,
                Role = user.Role
            }
        });
    }




    [HttpGet("profile")]
    [Authorize]
    public IActionResult GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            mensagem = "Perfil carregado com sucesso.",
            dados = new { UserId = userId, Name = name, Role = role }
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
            Role = u.Role
        });

        return Ok(new { mensagem = "Lista de usuários carregada com sucesso.", dados = response });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest dto)
    {
        var existing = await _userRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { mensagem = $"Usuário com ID {id} não foi encontrado." });

        existing.Name = dto.Name;
        existing.Role = dto.Role;

        _userRepository.Update(existing);
        await _userRepository.SaveChangesAsync();

        return Ok(new { mensagem = "Usuário atualizado com sucesso." });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { mensagem = $"Usuário com ID {id} não foi encontrado." });

        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync();

        return Ok(new { mensagem = "Usuário deletado com sucesso." });
    }

    private static string HashPassword(string password)
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 50000,
            numBytesRequested: 32);

        var saltBase64 = Convert.ToBase64String(salt);
        var hashBase64 = Convert.ToBase64String(hash);

        return $"{saltBase64}.{hashBase64}";
    }

    private static bool VerificarSenha(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword) || !hashedPassword.Contains("."))
            return false;

        var parts = hashedPassword.Split('.');
        if (parts.Length != 2)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var hashStored = Convert.FromBase64String(parts[1]);

        var hashToCompare = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 50000,
            numBytesRequested: 32);

        return hashStored.SequenceEqual(hashToCompare);
    }

    private string GenerateJwtToken(User user)
    {
        var secretKey = _config["JwtSettings:SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
            throw new InvalidOperationException("Chave JWT não configurada.");

        var issuer = _config["JwtSettings:Issuer"];
        var audience = _config["JwtSettings:Audience"];
        var expireMinutesString = _config["JwtSettings:ExpireMinutes"];

        if (string.IsNullOrEmpty(expireMinutesString) || !double.TryParse(expireMinutesString, out double expireMinutes))
            throw new InvalidOperationException("Configuração de tempo de expiração do JWT inválida.");

        var key = Encoding.UTF8.GetBytes(secretKey);
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Role, user.Role.ToString())
    };

        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }



}
