using CarniExpress.Auth.Data;
using CarniExpress.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace CarniExpress.Auth.Controllers;
[ApiController][Authorize][Route("api/perfil")]
public class PerfilController(CarniDbContext db):ControllerBase {
 private int? IdUsuario()=>int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),out var id)?id:null;
 [HttpGet]
 public async Task<IActionResult> Get(){var id=IdUsuario();if(id==null)return Unauthorized();var u=await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id && x.Activo);return u==null?NotFound(new {mensaje="Usuario no encontrado"}):Ok(new PerfilDto(u.Id,u.Nombre,u.Correo,u.Telefono,u.Rol));}
 [HttpPut]
 public async Task<IActionResult> Put(ActualizarPerfilRequest req){var id=IdUsuario();if(id==null)return Unauthorized();var u=await db.Usuarios.FirstOrDefaultAsync(x=>x.Id==id && x.Activo);if(u==null)return NotFound(new {mensaje="Usuario no encontrado"});u.Nombre=req.Nombre.Trim();u.Telefono=req.Telefono;await db.SaveChangesAsync();return Ok(new {mensaje="Perfil actualizado",usuario=new PerfilDto(u.Id,u.Nombre,u.Correo,u.Telefono,u.Rol)});}
}
