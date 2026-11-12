using CarniExpress.Auth.Data;
using CarniExpress.Auth.Models;
using CarniExpress.Auth.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CarniExpress.Auth.Controllers;
[ApiController][Route("api/auth")]
public class AuthController(CarniDbContext db,IPasswordHasher<Usuario> hasher,JwtService jwt) : ControllerBase {
 [HttpPost("login")]
 public async Task<IActionResult> Login(LoginRequest req){
  var correo=req.Correo.Trim().ToLowerInvariant();
  var u=await db.Usuarios.FirstOrDefaultAsync(x=>x.Correo==correo);
  if(u==null || !u.Activo || hasher.VerifyHashedPassword(u,u.PasswordHash,req.Contrasena)==PasswordVerificationResult.Failed)
   return Unauthorized(new {mensaje="Correo o contraseña incorrectos"});
  var (token,expires)=jwt.Crear(u);
  return Ok(new {token,expiraUtc=expires,usuario=new PerfilDto(u.Id,u.Nombre,u.Correo,u.Telefono,u.Rol)});
 }
}
