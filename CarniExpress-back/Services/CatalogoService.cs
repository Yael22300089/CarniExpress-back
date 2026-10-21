
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
                Categoria = "Res",
                PrecioPorKg = 180.00m,
                ImagenUrl = "/imagenes/costilla.jpg",
                ExistenciaKg = 0m,
                Activo = true
            },
            new Producto
            {
                Id = 3,
                Nombre = "Pechuga de pollo",
                Categoria = "Pollo",
                PrecioPorKg = 120.00m,
                ImagenUrl = "/imagenes/pechuga.jpg",
                ExistenciaKg = 18m,
                Activo = true
            },
            new Producto
            {
                Id = 4,
                Nombre = "Chuleta de cerdo",
                Categoria = "Cerdo",
                PrecioPorKg = 130.00m,
                ImagenUrl = "/imagenes/chuleta.jpg",
                ExistenciaKg = 12m,
                Activo = true
            },
            new Producto
            {
                Id = 5,
                Nombre = "Chorizo",
                Categoria = "Embutidos",
                PrecioPorKg = 140.00m,
                ImagenUrl = "/imagenes/chorizo.jpg",
                ExistenciaKg = 8m,
                Activo = true
            },
            new Producto
            {
                Id = 6,
                Nombre = "Producto desactivado",
                Categoria = "Res",
                PrecioPorKg = 100.00m,
                ImagenUrl = "",
                ExistenciaKg = 5m,
                Activo = false
            }
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
