
using CarniExpress_back.Models;
using CarniExpress_back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarniExpress_back.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/perfil")]
    public class PerfilController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public PerfilController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // =====================================
        // GET: api/perfil
        // CONSULTAR PERFIL
        // =====================================

        [HttpGet]
        public IActionResult ObtenerPerfil()
        {
            var idClaim = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idClaim, out int usuarioId))
            {
                return Unauthorized(new
                {
                    mensaje = "Sesión inválida"
                });
            }

            var usuario = _usuarioService.BuscarPorId(usuarioId);

            if (usuario == null || !usuario.Activo)
            {
                return Unauthorized(new
                {
                    mensaje = "Usuario no encontrado o inactivo"
                });
            }

            return Ok(new
            {
                mensaje = "Perfil consultado correctamente",
                usuario = new
                {
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Correo,
                    usuario.Telefono,
                    usuario.Rol
                }
            });
        }

        // =====================================
        // PUT: api/perfil
        // ACTUALIZAR PERFIL
        // =====================================

        [HttpPut]
        public IActionResult ActualizarPerfil(
            [FromBody] ActualizarPerfilRequest request)
        {
            var idClaim = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idClaim, out int usuarioId))
            {
                return Unauthorized(new
                {
                    mensaje = "Sesión inválida"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = _usuarioService.ActualizarPerfil(
                usuarioId, request);

            if (resultado.Error == "Este correo ya está registrado")
            {
                return Conflict(new
                {
                    mensaje = resultado.Error
                });
            }

            if (resultado.Usuario == null)
            {
                return BadRequest(new
                {
                    mensaje = resultado.Error
                });
            }

            var usuario = resultado.Usuario;

            return Ok(new
            {
                mensaje = "Perfil actualizado correctamente",
                usuario = new
                {
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Correo,
                    usuario.Telefono,
                    usuario.Rol
                }
            });
        }
    }
}
