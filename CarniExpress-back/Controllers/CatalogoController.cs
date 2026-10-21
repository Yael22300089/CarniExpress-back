
using CarniExpress_back.Models;
using CarniExpress_back.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarniExpress_back.Controllers
{
    [ApiController]
    [Route("api/catalogo")]
    [AllowAnonymous]
    public class CatalogoController : ControllerBase
    {
        private readonly CatalogoService _catalogoService;

        public CatalogoController(
            CatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
        }

        // =====================================
        // HU-005: CONSULTAR CATÁLOGO COMPLETO
        // GET: api/catalogo
        // =====================================

        [HttpGet]
        public IActionResult ObtenerCatalogo()
        {
            var productos =
                _catalogoService.ObtenerProductosActivos();

            if (productos.Count == 0)
            {
                return Ok(new
                {
                    mensaje =
                        "No hay productos disponibles por el momento",
                    totalProductos = 0,
                    categorias = Array.Empty<object>()
                });
            }

            var categorias = productos
                .GroupBy(p => p.Categoria)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    categoria = g.Key,
                    productos = g.ToList()
                })
                .ToList();

            return Ok(new
            {
                mensaje = "Catálogo consultado correctamente",
                totalProductos = productos.Count,
                categorias
            });
        }

        // =====================================
        // CONSULTAR UN PRODUCTO ACTIVO
        // GET: api/catalogo/1
        // =====================================

        [HttpGet("{id:int}")]
        public ActionResult<ProductoCatalogoDto>
            ObtenerProducto(int id)
        {
            var producto =
                _catalogoService.ObtenerPorId(id);

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = "Producto no encontrado"
                });
            }

            return Ok(producto);
        }
    }
}
