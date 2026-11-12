using CarniExpress.Auth.Data;
using CarniExpress.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CarniExpress.Auth.Controllers;
[ApiController][Route("api/registro")]
public class RegistroController(CarniDbContext db,IPasswordHasher<Usuario> hasher) : ControllerBase {
 [HttpPost]
 public async Task<IActionResult> Registrar(RegistroRequest req) {
  var correo=req.Correo.Trim().ToLowerInvariant();
  if(await db.Usuarios.AnyAsync(u=>u.Correo==correo)) return Conflict(new {mensaje="El correo ya está registrado"});
  var u=new Usuario{Nombre=req.Nombre.Trim(),Correo=correo,Telefono=req.Telefono,Rol="CLIENTE",Activo=true};
  u.PasswordHash=hasher.HashPassword(u,req.Contrasena);
  db.Usuarios.Add(u);
  try { await db.SaveChangesAsync(); } catch(DbUpdateException) {return Conflict(new {mensaje="No se pudo registrar. Verifica el correo"});}
  return StatusCode(201,new {mensaje="Cuenta registrada correctamente",id=u.Id});
 }
}
