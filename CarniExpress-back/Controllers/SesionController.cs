
using CarniExpress_back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace CarniExpress_back.Controllers
{
    [ApiController]
    [Route("api/sesion")]
    public class SesionController : ControllerBase
    {
        private readonly TokenRevocationService _tokens;

        public SesionController(TokenRevocationService tokens)
        {
            _tokens = tokens;
        }


        [Authorize]
        [HttpPost("cerrar")]
        public IActionResult CerrarSesion()
        {
            var tokenId = User.FindFirst("jti")?.Value;

            var token = HttpContext.Features
                .Get<Microsoft.AspNetCore.Authentication.JwtBearer
                    .JwtBearerChallengeContext>();

            if (string.IsNullOrWhiteSpace(tokenId))
            {
                return Unauthorized(new
                {
                    mensaje = "Token de sesión inválido"
                });
            }

            var expiracionClaim = User.FindFirst("exp")?.Value;

            if (!long.TryParse(expiracionClaim, out var exp))
            {
                return Unauthorized(new
                {
                    mensaje = "No se pudo determinar la expiración"
                });
            }

            _tokens.Revocar(
                tokenId,
                DateTimeOffset.FromUnixTimeSeconds(exp));

            return Ok(new
            {
                mensaje = "Sesión cerrada correctamente",
                redireccion = "login"
            });
        }

    }
}
