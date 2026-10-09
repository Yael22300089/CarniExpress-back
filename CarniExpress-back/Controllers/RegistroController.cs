using CarniExpress_back.Models;
using CarniExpress_back.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarniExpress_back.Controllers
{
    [ApiController]
    [Route("api/registro")]
    public class RegistroController : ControllerBase
    {
        private readonly UsuarioService _usuarios;
        private readonly IConfiguration _config;

        public RegistroController(
            UsuarioService usuarios,
            IConfiguration config)
        {
            _usuarios = usuarios;
            _config = config;
        }

        [HttpPost]
        public IActionResult Registrar([FromBody] RegistroRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Nombre) ||
                string.IsNullOrWhiteSpace(request.Correo) ||
                string.IsNullOrWhiteSpace(request.Telefono) ||
                string.IsNullOrWhiteSpace(request.Contrasena))
            {
                return BadRequest(new
                {
                    mensaje = "Todos los campos son obligatorios"
                });
            }

            var usuario = _usuarios.Registrar(request);

            if (usuario == null)
            {
                return Conflict(new
                {
                    mensaje = "Este correo ya está registrado"
                });
            }

            var secreto = _config["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "Falta configurar Jwt:Key");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secreto));

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha256));

            return StatusCode(201, new
            {
                mensaje = "Cuenta creada correctamente",
                token = new JwtSecurityTokenHandler()
                    .WriteToken(token),
                usuario = new
                {
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Correo,
                    usuario.Telefono,
                    usuario.Rol
                },
                redireccion = "catalogo"
            });
        }
    }
}