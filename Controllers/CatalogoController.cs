using CarniExpress.Auth.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CarniExpress.Auth.Controllers;
[ApiController][Route("api/catalogo")]
public class CatalogoController(CarniDbContext db):ControllerBase {
 [HttpGet] public async Task<IActionResult> Listar(){var datos=await db.Productos.AsNoTracking().Where(p=>p.Activo).OrderBy(p=>p.CategoriaId).ThenBy(p=>p.Nombre).Select(p=>new {p.Id,p.Nombre,p.Descripcion,Categoria=p.Categoria.Nombre,p.PrecioPorKg,p.ImagenUrl,p.ExistenciaKg,Disponible=p.ExistenciaKg>0}).ToListAsync();return Ok(datos);}
 [HttpGet("{id:int}")] public async Task<IActionResult> Detalle(int id){var p=await db.Productos.AsNoTracking().Where(p=>p.Id==id&&p.Activo).Select(p=>new {p.Id,p.Nombre,p.Descripcion,Categoria=p.Categoria.Nombre,p.PrecioPorKg,p.ImagenUrl,p.ExistenciaKg,Disponible=p.ExistenciaKg>0,PuedeAgregar=p.ExistenciaKg>=0.5m,Estado=p.ExistenciaKg>=0.5m?"Disponible":"No disponible"}).FirstOrDefaultAsync();return p==null?NotFound(new{mensaje="Producto no encontrado o no disponible"}):Ok(new{mensaje="Detalle consultado correctamente",producto=p});}
}
