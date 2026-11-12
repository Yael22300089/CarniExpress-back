using CarniExpress.Auth.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace CarniExpress.Auth.Services;
public class JwtService(IConfiguration config) {
 public (string token,DateTime expires) Crear(Usuario usuario) {
  var key = config["Jwt:Key"] ?? throw new InvalidOperationException("Falta Jwt:Key");
  var expires=DateTime.UtcNow.AddMinutes(int.Parse(config["Jwt:Minutes"] ?? "60"));
  var claims = new[] {new Claim(ClaimTypes.NameIdentifier,usuario.Id.ToString()),new Claim(ClaimTypes.Email,usuario.Correo),new Claim(ClaimTypes.Role,usuario.Rol),new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())};
  var token=new JwtSecurityToken(issuer:config["Jwt:Issuer"],audience:config["Jwt:Audience"],claims:claims,expires:expires,signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),SecurityAlgorithms.HmacSha256));
  return (new JwtSecurityTokenHandler().WriteToken(token),expires);
 }
}
