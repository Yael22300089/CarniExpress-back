using CarniExpress.Auth.Data;
using CarniExpress.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
namespace CarniExpress.Auth.Controllers;
[ApiController][Authorize][Route("api/sesion")]
public class SesionController(CarniDbContext db):ControllerBase {
 [HttpPost("cerrar")]
 public async Task<IActionResult> Cerrar(){
  var jti=User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value ?? User.FindFirst("jti")?.Value;
  var exp=User.FindFirst("exp")?.Value;
  if(string.IsNullOrEmpty(jti)||!long.TryParse(exp,out var unix))return Unauthorized(new {mensaje="Token inválido"});
  if(await db.TokensRevocados.FindAsync(jti)==null){db.TokensRevocados.Add(new TokenRevocado{Jti=jti,FechaExpiracionUtc=DateTimeOffset.FromUnixTimeSeconds(unix).UtcDateTime});await db.SaveChangesAsync();}
  return Ok(new {mensaje="Sesión cerrada"});
 }
}
