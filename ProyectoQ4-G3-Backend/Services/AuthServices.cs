using System.Security.Cryptography;
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
            {"Email", user.Email},
            {"PasswhordHash", user.PasswordHash},
            {"CreateAt", user.CreatedAt}
        });
        return user;
    }

    private string HashPassword(string password)
    {
        // SHA256
        var bytes = SHA256.HashData(Enconding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
    
}