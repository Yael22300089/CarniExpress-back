namespace CarniExpress.Auth.Models;
public class Categoria { public int Id {get;set;} public string Nombre {get;set;}=""; public ICollection<Producto> Productos {get;set;}=new List<Producto>(); }
public class Producto { public int Id {get;set;} public string Nombre {get;set;}=""; public string Descripcion {get;set;}=""; public int CategoriaId {get;set;} public Categoria Categoria {get;set;}=null!; public decimal PrecioPorKg {get;set;} public decimal ExistenciaKg {get;set;} public string ImagenUrl {get;set;}=""; public bool Activo {get;set;}=true; }
public class Carrito { public int Id {get;set;} public int UsuarioId {get;set;} public Usuario Usuario {get;set;}=null!; public DateTime CreadoUtc {get;set;}=DateTime.UtcNow; public ICollection<CarritoItem> Items {get;set;}=new List<CarritoItem>(); }
public class CarritoItem { public int Id {get;set;} public int CarritoId {get;set;} public Carrito Carrito {get;set;}=null!; public int ProductoId {get;set;} public Producto Producto {get;set;}=null!; public decimal CantidadKg {get;set;} public decimal PrecioPorKg {get;set;} }
public record AgregarCarritoRequest(int ProductoId, decimal CantidadKg);
