using CarniExpress.Auth.Models;
using Microsoft.EntityFrameworkCore;
namespace CarniExpress.Auth.Data;
public class CarniDbContext(DbContextOptions<CarniDbContext> options) : DbContext(options) {
 public DbSet<Categoria> Categorias => Set<Categoria>();
 public DbSet<Producto> Productos => Set<Producto>();
 public DbSet<Carrito> Carritos => Set<Carrito>();
 public DbSet<CarritoItem> CarritoItems => Set<CarritoItem>();
 public DbSet<Usuario> Usuarios => Set<Usuario>();
 public DbSet<TokenRevocado> TokensRevocados => Set<TokenRevocado>();
 protected override void OnModelCreating(ModelBuilder m) {
  m.Entity<Categoria>(e=>{e.ToTable("Categorias");e.HasKey(x=>x.Id);e.Property(x=>x.Nombre).HasMaxLength(50).IsRequired();e.HasIndex(x=>x.Nombre).IsUnique();});
  m.Entity<Producto>(e=>{e.ToTable("Productos");e.HasKey(x=>x.Id);e.Property(x=>x.Nombre).HasMaxLength(120).IsRequired();e.Property(x=>x.Descripcion).HasMaxLength(1000).IsRequired();e.Property(x=>x.ImagenUrl).HasMaxLength(500).IsRequired();e.Property(x=>x.PrecioPorKg).HasPrecision(12,2);e.Property(x=>x.ExistenciaKg).HasPrecision(12,2);e.HasOne(x=>x.Categoria).WithMany(x=>x.Productos).HasForeignKey(x=>x.CategoriaId).OnDelete(DeleteBehavior.Restrict);});
  m.Entity<Carrito>(e=>{e.ToTable("Carritos");e.HasKey(x=>x.Id);e.HasIndex(x=>x.UsuarioId).IsUnique();e.HasOne(x=>x.Usuario).WithMany().HasForeignKey(x=>x.UsuarioId).OnDelete(DeleteBehavior.Cascade);});
  m.Entity<CarritoItem>(e=>{e.ToTable("CarritoItems");e.HasKey(x=>x.Id);e.Property(x=>x.CantidadKg).HasPrecision(12,2);e.Property(x=>x.PrecioPorKg).HasPrecision(12,2);e.HasIndex(x=>new{x.CarritoId,x.ProductoId}).IsUnique();e.HasOne(x=>x.Carrito).WithMany(x=>x.Items).HasForeignKey(x=>x.CarritoId).OnDelete(DeleteBehavior.Cascade);e.HasOne(x=>x.Producto).WithMany().HasForeignKey(x=>x.ProductoId).OnDelete(DeleteBehavior.Restrict);});
  m.Entity<Usuario>(e=>{
   e.ToTable("Usuarios");e.HasKey(x=>x.Id);e.Property(x=>x.Id).ValueGeneratedOnAdd();
   e.Property(x=>x.Nombre).HasMaxLength(120).IsRequired();
   e.Property(x=>x.Correo).HasMaxLength(256).IsRequired();e.HasIndex(x=>x.Correo).IsUnique();
   e.Property(x=>x.Telefono).HasMaxLength(10).IsRequired();e.Property(x=>x.PasswordHash).IsRequired();
   e.Property(x=>x.Rol).HasMaxLength(20).IsRequired();
   e.Property(x=>x.FechaRegistro).HasDefaultValueSql("SYSUTCDATETIME()");
  });
  m.Entity<TokenRevocado>(e=>{e.ToTable("TokensRevocados");e.HasKey(x=>x.Jti);e.Property(x=>x.Jti).HasMaxLength(100);e.Property(x=>x.FechaRevocacionUtc).HasDefaultValueSql("SYSUTCDATETIME()");});
 }
}
