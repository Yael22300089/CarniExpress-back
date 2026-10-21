
using CarniExpress_back.Models;

namespace CarniExpress_back.Services
{
    public class CatalogoService
    {
        private readonly List<Producto> _productos = new()
        {

new Producto
{
    Id = 1,
    Nombre = "Bistec de res",
    Descripcion = "Corte de carne de res ideal para asar o preparar a la plancha. Se vende por kilogramo.",
    Categoria = "Res",
    PrecioPorKg = 210.00m,
    ImagenUrl = "/imagenes/bistec.jpg",
    ExistenciaKg = 25m,
    Activo = true
},
new Producto
{
    Id = 2,
    Nombre = "Costilla de res",
    Descripcion = "Costilla de res para preparar al horno, a la parrilla o en guisados.",
    Categoria = "Res",
    PrecioPorKg = 180.00m,
    ImagenUrl = "/imagenes/costilla.jpg",
    ExistenciaKg = 0m,
    Activo = true
},

        };

        public List<ProductoCatalogoDto> ObtenerProductosActivos()
        {
            return _productos
                .Where(p => p.Activo)
                .OrderBy(p => p.Categoria)
                .ThenBy(p => p.Nombre)
                .Select(p => new ProductoCatalogoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Categoria = p.Categoria,
                    PrecioPorKg = p.PrecioPorKg,
                    ImagenUrl = p.ImagenUrl,
                    Disponible = p.ExistenciaKg > 0,
                    PuedeAgregar = p.ExistenciaKg > 0
                })
                .ToList();
        }

        public ProductoCatalogoDto? ObtenerPorId(int id)
        {
            return ObtenerProductosActivos()
                .FirstOrDefault(p => p.Id == id);
        }


        public List<ProductoCatalogoDto> BuscarPorNombre(string? texto)
        {
            var productos = ObtenerProductosActivos();

            if (string.IsNullOrWhiteSpace(texto))
            {
                return productos;
            }

            string busqueda = NormalizarTexto(texto.Trim());

            return productos
                .Where(p =>
                    NormalizarTexto(p.Nombre).Contains(
                        busqueda,
                        StringComparison.Ordinal))
                .ToList();
        }


        public List<ProductoCatalogoDto> FiltrarPorCategoria(
            string categoria)
        {
            // Obtener solamente productos activos
            var productos = ObtenerProductosActivos();

            // Filtrar por la categoría seleccionada
            return productos
                .Where(p => p.Categoria.Equals(
                    categoria,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
        }


        public ProductoDetalleDto? ObtenerDetallePorId(int id)
        {
            // Consultar el producto activo
            var producto = _productos.FirstOrDefault(p =>
                p.Id == id && p.Activo);

            if (producto == null)
            {
                return null;
            }

            // Comprobar disponibilidad
            bool disponible = producto.ExistenciaKg > 0;

            // Devolver información actual del producto
            return new ProductoDetalleDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Categoria = producto.Categoria,
                PrecioPorKg = producto.PrecioPorKg,
                ImagenUrl = producto.ImagenUrl,
                ExistenciaKg = producto.ExistenciaKg,
                Disponible = disponible,
                PuedeAgregar = disponible,
                Estado = disponible
                    ? "Disponible"
                    : "No disponible"
            };
        }



        private static string NormalizarTexto(string texto)
        {
            string normalizado = texto.Normalize(
                System.Text.NormalizationForm.FormD);

            var resultado = new System.Text.StringBuilder();

            foreach (char caracter in normalizado)
            {
                var categoria =
                    System.Globalization.CharUnicodeInfo.GetUnicodeCategory(
                        caracter);

                if (categoria !=
                    System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    resultado.Append(char.ToLowerInvariant(caracter));
                }
            }

            return resultado.ToString()
                .Normalize(System.Text.NormalizationForm.FormC);
        }

    }
}
