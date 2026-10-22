
using CarniExpress_back.Models;
using CarniExpress_back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CarniExpress_back.Controllers
{
    [ApiController]
    [Route("api/carrito")]
    [Authorize(Roles = "CLIENTE")]
    public class CarritoController : ControllerBase
    {
        private readonly CarritoService _carrito;

        public CarritoController(CarritoService carrito)
        {
            _carrito = carrito;
        }

        // Identificar sesión desde el JWT
        private string? ObtenerSesionId()
        {
            var usuarioId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var tokenId = User.FindFirstValue(
                JwtRegisteredClaimNames.Jti)
                ?? User.FindFirstValue("jti");

            if (string.IsNullOrWhiteSpace(usuarioId) ||
                string.IsNullOrWhiteSpace(tokenId))
            {
                return null;
            }

            return $"{usuarioId}:{tokenId}";
        }

        // POST: api/carrito/productos
        [HttpPost("productos")]
        public IActionResult AgregarProducto(
            [FromBody] AgregarCarritoRequest request)
        {
            var sesionId = ObtenerSesionId();

            if (sesionId == null)
                return Unauthorized(new
                {
                    mensaje = "Sesión no válida"
                });

            var resultado = _carrito.Agregar(
                sesionId, request);

            if (!resultado.Exito)
            {
                return BadRequest(new
                {
                    mensaje = resultado.Mensaje
                });
            }

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                carrito = _carrito.ObtenerCarrito(sesionId)
            });
        }

        // GET: api/carrito
        [HttpGet]
        public IActionResult ConsultarCarrito()
        {
            var sesionId = ObtenerSesionId();

            if (sesionId == null)
                return Unauthorized(new
                {
                    mensaje = "Sesión no válida"
                });

            return Ok(_carrito.ObtenerCarrito(sesionId));
        }

        // GET: api/carrito/indicador
        [HttpGet("indicador")]
        public IActionResult ConsultarIndicador()
        {
            var sesionId = ObtenerSesionId();

            if (sesionId == null)
                return Unauthorized(new
                {
                    mensaje = "Sesión no válida"
                });

            return Ok(new
            {
                totalProductos =
                    _carrito.ObtenerIndicador(sesionId)
            });
        }




        // =====================================
        // HU-010: AUMENTAR CANTIDAD
        // PATCH: api/carrito/productos/1/aumentar
        // =====================================

        [HttpPatch("productos/{id:int}/aumentar")]
        public IActionResult AumentarCantidad(int id)
        {
            if (id <= 0)
                return BadRequest(new
                {
                    mensaje = "Producto no válido"
                });

            var sesionId = ObtenerSesionId();

            if (sesionId == null)
                return Unauthorized(new
                {
                    mensaje = "Sesión no válida"
                });

            var resultado = _carrito.AumentarCantidad(sesionId, id);

            if (!resultado.Exito)
                return BadRequest(new
                {
                    mensaje = resultado.Mensaje
                });

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                carrito = _carrito.ObtenerCarrito(sesionId)
            });
        }


        // =====================================
        // HU-010: DISMINUIR CANTIDAD
        // PATCH: api/carrito/productos/1/disminuir
        // =====================================

        [HttpPatch("productos/{id:int}/disminuir")]
        public IActionResult DisminuirCantidad(int id)
        {
            if (id <= 0)
                return BadRequest(new
                {
                    mensaje = "Producto no válido"
                });

            var sesionId = ObtenerSesionId();

            if (sesionId == null)
                return Unauthorized(new
                {
                    mensaje = "Sesión no válida"
                });

            var resultado = _carrito.DisminuirCantidad(sesionId, id);

            if (!resultado.Exito)
                return BadRequest(new
                {
                    mensaje = resultado.Mensaje
                });

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                carrito = _carrito.ObtenerCarrito(sesionId)
            });
        }


        // ==========================================
        // HU-011: ELIMINAR PRODUCTO DEL CARRITO
        // DELETE: api/carrito/productos/1?confirmar=true
        // ==========================================

        [HttpDelete("productos/{id:int}")]
        public IActionResult EliminarProducto(
            int id,
            [FromQuery] bool confirmar = false)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Producto no válido"
                });
            }

            // Exigir confirmación explícita
            if (!confirmar)
            {
                return BadRequest(new
                {
                    mensaje = "Debes confirmar la eliminación"
                });
            }

            // Identificar al cliente autenticado
            var sesionId = ObtenerSesionId();

            if (string.IsNullOrWhiteSpace(sesionId))
            {
                return Unauthorized(new
                {
                    mensaje = "Sesión no válida"
                });
            }

            // Eliminar producto
            var resultado = _carrito.EliminarProducto(
                sesionId, id);

            if (!resultado.Exito)
            {
                return BadRequest(new
                {
                    mensaje = resultado.Mensaje
                });
            }

            // Obtener carrito actualizado
            var carrito = _carrito.ObtenerCarrito(sesionId);

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                carrito
            });
        }


    }
}
