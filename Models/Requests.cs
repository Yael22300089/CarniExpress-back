using System.ComponentModel.DataAnnotations;
namespace CarniExpress.Auth.Models;
public class RegistroRequest {
 [Required, MaxLength(120)] public string Nombre {get;set;} = "";
 [Required,EmailAddress,MaxLength(256)] public string Correo {get;set;} = "";
 [Required,RegularExpression(@"^[0-9]{10}$")] public string Telefono {get;set;} = "";
 [Required,MinLength(8)] public string Contrasena {get;set;} = "";
}
public class LoginRequest {
 [Required,EmailAddress] public string Correo {get;set;} = "";
 [Required] public string Contrasena {get;set;} = "";
}
public class ActualizarPerfilRequest {
 [Required,MaxLength(120)] public string Nombre {get;set;} = "";
 [Required,RegularExpression(@"^[0-9]{10}$")] public string Telefono {get;set;} = "";
}
public record PerfilDto(int Id,string Nombre,string Correo,string Telefono,string Rol);
