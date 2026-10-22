
using CarniExpress_back.Models;

namespace CarniExpress_back.Services
{
    public class CarritoService
    {
        private readonly CatalogoService _catalogo;

        private readonly Dictionary<string, List<CarritoItem>>
            _carritos = new();

        private readonly object _lock = new();

        public CarritoService(CatalogoService catalogo)
        {
            _catalogo = catalogo;
        }

        // HU-009: AGREGAR PRODUCTO
        public (bool Exito, string Mensaje) Agregar(
            string sesionId,
            AgregarCarritoRequest request)
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(sesionId))
                    return (false, "Sesión no válida");

                if (request.CantidadKg < 0.5m)
                    return (false,
                        "La cantidad mínima es 0.5 kg");

                var producto = _catalogo.ObtenerDetallePorId(
                    request.ProductoId);

                if (producto == null ||
                    !producto.Disponible)
                {
                    return (false,
                        "Producto no disponible");
                }

                if (!_carritos.TryGetValue(
                    sesionId, out var items))
                {
                    items = new List<CarritoItem>();
                    _carritos[sesionId] = items;
                }

                var existente = items.FirstOrDefault(
                    p => p.ProductoId == request.ProductoId);

                decimal cantidadActual =
                    existente?.CantidadKg ?? 0m;

                decimal cantidadTotal;

                try
                {
                    cantidadTotal = checked(
                        cantidadActual + request.CantidadKg);
                }
                catch (OverflowException)
                {
                    return (false, "Cantidad no válida");
                }

                if (cantidadTotal > producto.ExistenciaKg)
                {
                    return (false,
                        "Existencia insuficiente");
                }

                if (existente != null)
                {
                    // Sumar sin duplicar el producto
                    existente.CantidadKg = cantidadTotal;
                    existente.PrecioPorKg =
                        producto.PrecioPorKg;
                    existente.Nombre = producto.Nombre;
                }
                else
                {
                    items.Add(new CarritoItem
                    {
                        ProductoId = producto.Id,
                        Nombre = producto.Nombre,
                        CantidadKg = request.CantidadKg,
                        PrecioPorKg = producto.PrecioPorKg
                    });
                }

                return (true,
                    "Producto agregado correctamente");
            }
        }

        // HU-009: CONSULTAR CARRITO
        public object ObtenerCarrito(string sesionId)
        {
            lock (_lock)
            {
                var items = _carritos.TryGetValue(
                    sesionId, out var carrito)
                    ? carrito
                    : new List<CarritoItem>();

                // Crear copias para no exponer
                // los objetos internos del servicio
                var productos = items.Select(p =>
                    new CarritoItem
                    {
                        ProductoId = p.ProductoId,
                        Nombre = p.Nombre,
                        CantidadKg = p.CantidadKg,
                        PrecioPorKg = p.PrecioPorKg
                    }).ToList();

                return new
                {
                    productos,
                    totalProductos = productos.Count,
                    totalKg = productos.Sum(p => p.CantidadKg),
                    total = productos.Sum(p => p.Subtotal)
                };
            }
        }

        // HU-009: INDICADOR DEL ENCABEZADO
        public int ObtenerIndicador(string sesionId)
        {
            lock (_lock)
            {
                return _carritos.TryGetValue(
                    sesionId, out var carrito)
                    ? carrito.Count
                    : 0;
            }
        }

        //HU-010 Aumentar 0.5 en el carrito 
        public (bool Exito, string Mensaje) AumentarCantidad(
            string sesionId, int productoId)
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(sesionId))
                    return (false, "Sesión no válida");

                if (!_carritos.TryGetValue(sesionId, out var items))
                    return (false, "Carrito no encontrado");

                var item = items.FirstOrDefault(
                    p => p.ProductoId == productoId);

                if (item == null)
                    return (false, "Producto no encontrado en el carrito");

                var producto = _catalogo.ObtenerDetallePorId(productoId);

                if (producto == null || !producto.Disponible)
                    return (false, "Producto no disponible");

                decimal nuevaCantidad = item.CantidadKg + 0.5m;

                if (nuevaCantidad > producto.ExistenciaKg)
                {
                    return (false,
                        "No hay más existencia disponible");
                }

                item.CantidadKg = nuevaCantidad;
                item.PrecioPorKg = producto.PrecioPorKg;
                item.Nombre = producto.Nombre;

                return (true, "Cantidad aumentada correctamente");
            }
        }


        // HU-010 Disminuir 0.5 en el carrito 
        public (bool Exito, string Mensaje) DisminuirCantidad(
            string sesionId, int productoId)
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(sesionId))
                    return (false, "Sesión no válida");

                if (!_carritos.TryGetValue(sesionId, out var items))
                    return (false, "Carrito no encontrado");

                var item = items.FirstOrDefault(
                    p => p.ProductoId == productoId);

                if (item == null)
                    return (false, "Producto no encontrado en el carrito");

                decimal nuevaCantidad = item.CantidadKg - 0.5m;

                // Eliminar automáticamente al bajar de 0.5 kg
                if (nuevaCantidad < 0.5m)
                {
                    items.Remove(item);

                    if (items.Count == 0)
                        _carritos.Remove(sesionId);

                    return (true, "Producto eliminado del carrito");
                }

                item.CantidadKg = nuevaCantidad;

                // Actualizar el precio si el producto sigue activo
                var producto = _catalogo.ObtenerDetallePorId(productoId);

                if (producto != null)
                {
                    item.PrecioPorKg = producto.PrecioPorKg;
                    item.Nombre = producto.Nombre;
                }

                return (true, "Cantidad disminuida correctamente");
            }
        }



        // ==========================================
        // HU-011: ELIMINAR PRODUCTO DEL CARRITO
        // ==========================================

        public (bool Exito, string Mensaje) EliminarProducto(
            string sesionId, int productoId)
        {
            lock (_lock)
            {
                // Validar la sesión
                if (string.IsNullOrWhiteSpace(sesionId))
                {
                    return (false, "Sesión no válida");
                }

                // Validar identificador
                if (productoId <= 0)
                {
                    return (false, "Producto no válido");
                }

                // Buscar el carrito de la sesión
                if (!_carritos.TryGetValue(sesionId, out var items))
                {
                    return (false, "Carrito no encontrado");
                }

                // Buscar el producto seleccionado
                var item = items.FirstOrDefault(
                    p => p.ProductoId == productoId);

                if (item == null)
                {
                    return (false,
                        "Producto no encontrado en el carrito");
                }

                // Eliminar únicamente el producto solicitado
                items.Remove(item);

                // Si ya no quedan productos,
                // eliminar el carrito vacío de memoria
                if (items.Count == 0)
                {
                    _carritos.Remove(sesionId);
                }

                return (true, "Producto eliminado correctamente");
            }
        }



    }
}
