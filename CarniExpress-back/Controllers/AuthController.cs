using Microsoft.AspNetCore.Mvc;
using CarniExpress_back.Models;

namespace CarniExpress_back.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Correo) ||
                string.IsNullOrWhiteSpace(request.Contrasena))
            {
                return BadRequest(new
                {
                    mensaje = "Todos los campos son obligatorios"
                });
            }

            var usuarios = new[]
            {
                new {
                    Correo = "cliente@carniexpress.com",
                    Contrasena = "Cliente123",
                    Rol = "CLIENTE",
                    Activo = true
                },
                new {
                    Correo = "empleado@carniexpress.com",
                    Contrasena = "Empleado123",
                    Rol = "EMPLEADO",
                    Activo = true
                },
                new {
                    Correo = "repartidor@carniexpress.com",
                    Contrasena = "Repartidor123",
                    Rol = "REPARTIDOR",
                    Activo = true
                }
            };

            var usuario = usuarios.FirstOrDefault(u =>
                u.Correo.Equals(
                    request.Correo.Trim(),
                    StringComparison.OrdinalIgnoreCase) &&
                u.Contrasena == request.Contrasena);

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensaje = "Correo o contraseña incorrectos"
                });
            }

            if (!usuario.Activo)
            {
                return StatusCode(403, new
                {
                    mensaje = "Usuario inactivo"
                });
            }

            string pantalla = usuario.Rol switch
            {
                "CLIENTE" => "catalogo",
                "EMPLEADO" => "dashboard",
                "REPARTIDOR" => "entregas",
                _ => "inicio"
            };

            return Ok(new
            {
                mensaje = "Inicio de sesión correcto",
                correo = usuario.Correo,
                rol = usuario.Rol,
                redireccion = pantalla
            });
        }
    }
}