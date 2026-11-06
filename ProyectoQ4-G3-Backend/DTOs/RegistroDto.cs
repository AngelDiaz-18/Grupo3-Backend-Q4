namespace ProyectoQ4_G3_Backend.DTOs;
 
// DTO Significa Data Transfer Object
//* Es una clase que describe SOLO los datos dque el FE debe enviar
//* ID o la fecha las debe asignar - calcular el servidor

public class RegistroDto
{
    public string FullName { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
}