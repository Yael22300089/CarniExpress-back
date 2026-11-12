namespace CarniExpress.Auth.Models;
public class Usuario {
 public int Id {get;set;}
 public string Nombre {get;set;} = "";
 public string Correo {get;set;} = "";
 public string Telefono {get;set;} = "";
 public string PasswordHash {get;set;} = "";
 public string Rol {get;set;} = "CLIENTE";
 public bool Activo {get;set;} = true;
 public DateTime FechaRegistro {get;set;} = DateTime.UtcNow;
}
public class TokenRevocado {
 public string Jti {get;set;} = "";
 public DateTime FechaExpiracionUtc {get;set;}
 public DateTime FechaRevocacionUtc {get;set;} = DateTime.UtcNow;
}
