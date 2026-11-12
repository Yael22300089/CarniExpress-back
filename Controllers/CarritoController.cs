using CarniExpress.Auth.Data;
using CarniExpress.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace CarniExpress.Auth.Controllers;
[ApiController][Route("api/carrito")][Authorize(Roles="CLIENTE")]
public class CarritoController(CarniDbContext db):ControllerBase {
 private int UsuarioId=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
 private async Task<object> Resumen(){var items=await db.CarritoItems.AsNoTracking().Where(x=>x.Carrito.UsuarioId==UsuarioId).Select(x=>new{x.ProductoId,x.Producto.Nombre,x.CantidadKg,x.PrecioPorKg}).ToListAsync();var productos=items.Select(x=>new{x.ProductoId,x.Nombre,x.CantidadKg,x.PrecioPorKg,Subtotal=decimal.Round(x.CantidadKg*x.PrecioPorKg,2,MidpointRounding.AwayFromZero)}).ToList();return new{productos,totalProductos=productos.Count,totalKg=productos.Sum(x=>x.CantidadKg),total=productos.Sum(x=>x.Subtotal)};}
 [HttpGet] public async Task<IActionResult> Consultar()=>Ok(await Resumen());
 [HttpGet("indicador")] public async Task<IActionResult> Indicador()=>Ok(new{totalProductos=await db.CarritoItems.CountAsync(x=>x.Carrito.UsuarioId==UsuarioId)});
 [HttpPost("productos")] public async Task<IActionResult> Agregar([FromBody]AgregarCarritoRequest req){
  if(req.ProductoId<=0||req.CantidadKg<0.5m||req.CantidadKg!=decimal.Round(req.CantidadKg,2))return BadRequest(new{mensaje="Producto inválido o cantidad inferior a 0.5 kg (máximo dos decimales)"});
  // Transacción serializable para evitar actualizaciones concurrentes del mismo carrito.
  await using var tx=await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
  var producto=await db.Productos.FirstOrDefaultAsync(x=>x.Id==req.ProductoId&&x.Activo);
  if(producto==null||producto.ExistenciaKg<0.5m)return BadRequest(new{mensaje="Producto no disponible"});
  var carrito=await db.Carritos.Include(x=>x.Items).FirstOrDefaultAsync(x=>x.UsuarioId==UsuarioId);
  if(carrito==null){carrito=new Carrito{UsuarioId=UsuarioId};db.Carritos.Add(carrito);}
  var item=carrito.Items.FirstOrDefault(x=>x.ProductoId==req.ProductoId);
  decimal cantidad;try{cantidad=checked((item?.CantidadKg??0m)+req.CantidadKg);}catch(OverflowException){return BadRequest(new{mensaje="Cantidad no válida"});}
  if(cantidad>producto.ExistenciaKg)return BadRequest(new{mensaje="Existencia insuficiente"});
  if(item==null)carrito.Items.Add(new CarritoItem{ProductoId=req.ProductoId,CantidadKg=cantidad,PrecioPorKg=producto.PrecioPorKg});
  else{item.CantidadKg=cantidad;item.PrecioPorKg=producto.PrecioPorKg;}
  await db.SaveChangesAsync();await tx.CommitAsync();return Ok(new{mensaje="Producto agregado correctamente",carrito=await Resumen()});
 }

 // HU-010: ajustar cantidad en pasos de 0.5 kg. La disminución por debajo de 0.5 elimina el artículo.
 [HttpPatch("productos/{id:int}/aumentar")]
 public Task<IActionResult> Aumentar(int id) => Ajustar(id, 0.5m);

 [HttpPatch("productos/{id:int}/disminuir")]
 public Task<IActionResult> Disminuir(int id) => Ajustar(id, -0.5m);

 private async Task<IActionResult> Ajustar(int id, decimal cambio)
 {
  if(id<=0) return BadRequest(new{mensaje="Producto no válido"});
  await using var tx=await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
  var item=await db.CarritoItems.Include(x=>x.Producto)
   .FirstOrDefaultAsync(x=>x.ProductoId==id && x.Carrito.UsuarioId==UsuarioId);
  if(item==null) return NotFound(new{mensaje="Producto no encontrado en el carrito"});
  var nuevaCantidad=item.CantidadKg+cambio;
  if(cambio>0 && (!item.Producto.Activo || nuevaCantidad>item.Producto.ExistenciaKg))
   return BadRequest(new{mensaje="No hay más existencia disponible"});
  if(nuevaCantidad<0.5m) db.CarritoItems.Remove(item);
  else { item.CantidadKg=nuevaCantidad; item.PrecioPorKg=item.Producto.PrecioPorKg; }
  await db.SaveChangesAsync();
  await tx.CommitAsync();
  return Ok(new{mensaje=nuevaCantidad<0.5m?"Producto eliminado del carrito":"Cantidad actualizada correctamente",carrito=await Resumen()});
 }

 // HU-011: la interfaz solicita confirmación; la API también exige confirmación explícita.
 [HttpDelete("productos/{id:int}")]
 public async Task<IActionResult> Eliminar(int id, [FromQuery] bool confirmar=false)
 {
  if(id<=0) return BadRequest(new{mensaje="Producto no válido"});
  if(!confirmar) return BadRequest(new{mensaje="Debes confirmar la eliminación"});
  await using var tx=await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
  var item=await db.CarritoItems.FirstOrDefaultAsync(x=>x.ProductoId==id && x.Carrito.UsuarioId==UsuarioId);
  if(item==null) return NotFound(new{mensaje="Producto no encontrado en el carrito"});
  db.CarritoItems.Remove(item);
  await db.SaveChangesAsync();
  await tx.CommitAsync();
  return Ok(new{mensaje="Producto eliminado correctamente",carrito=await Resumen()});
 }

 // HU-012: mismo cálculo de subtotales y total que GET /api/carrito.
 [HttpGet("desglose")]
 public async Task<IActionResult> Desglose()
 {
  var resumen=await Resumen();
  return Ok(resumen);
 }
}
