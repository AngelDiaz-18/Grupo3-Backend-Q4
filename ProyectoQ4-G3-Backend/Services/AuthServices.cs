using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProyectoQ4_G3_Backend.DTOs;
using ProyectoQ4_G3_Backend.Models;
 

namespace ProyectoQ4_G3_Backend.Services;

public class AuthServices
{
    private readonly FirebaseService _firebaseService; 
    
    private readonly IConfiguration _configuration;
    
    public AuthServices(FirebaseService firebaseService, IConfiguration configuration)
        {
        _firebaseService = firebaseService;
        _configuration = configuration;
        }

    public async Task<User> Register(RegistroDto dto)
    {
        var collection = _firebaseService.GetCollection("users");


        // Antes de crar, buscar el correo (que no exista)
        var existing = await collection
            .WhereEqualTo("Email", dto.Email)
            .GetSnapshotAsync();

        if (existing.Count > 0)
        {
            throw new Exception("Ya existe un usuario con ese correo");
        }

        var user = new User()
        {
            Id = Guid.NewGuid().ToString(),
            FullName = dto.FullName,
            Email = dto.Email,
            // Hash y CreateAt
            PasswordHash = HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow,
        };

        await collection.Document(user.Id).SetAsync(new Dictionary<string, object>
        {
            {"ID", user.Id },
            {"FullName", user.FullName },
            {"Email", user.Email},
            {"PasswhordHash", user.PasswordHash},
            {"CreateAt", user.CreatedAt}
        });
        return user;
    }

    public async Task<string> Login(LoginDto dto)
    {

        var collection = _firebaseService.GetCollection("user");
        
        // Antes de crar, buscar el correo (que no exista)
        var snapshot = await collection
            .WhereEqualTo("Email", dto.Email)
            .GetSnapshotAsync();
        
        if (snapshot.Count == 0)
        {
            throw new Exception("Credenciales Invalidas"); 
        }

        var data = snapshot.Documents[0].ToDictionary();

        var user = new User()
        {
            Id = data["Id"].ToString()!,
            FullName = data["FullName"].ToString()!,
            Email = data["Email"].ToString()!,
            PasswordHash = data["PasswordHash"].ToString()!,
            CreatedAt = ((Google.Cloud.Firestore.Timestamp)data["CreatedAt"]).ToDateTime()
        };

        if (!VerifyPassword(dto.Password, user.PasswordHash))
            throw new Exception("Credenciales Invalidas");

        return GenerateToken(user);

    }

    private string GenerateToken(User user)
    {
        //Claims - Son los datos que viajan dentro del token
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
        };
        
        //Clave secreta firma el token, si alguien lo atltera, el servidor lo rechaza.
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"], //quien genera el token?
            audience: _configuration["Jwt:Issuer"], // para quien es el token?
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    private bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }

    private string HashPassword(string password)
    {
        // SHA256
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
    
}

