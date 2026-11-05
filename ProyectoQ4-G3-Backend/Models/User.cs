namespace ProyectoQ4_G3_Backend.Models;

public class User
{
    public strind Id { get: set; } = string.Empty;
    
    public string FullName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
}