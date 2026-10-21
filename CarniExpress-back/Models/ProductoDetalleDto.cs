
namespace CarniExpress_back.Models
{
    public class ProductoDetalleDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = "";

        public string Descripcion { get; set; } = "";

        public string Categoria { get; set; } = "";

        public decimal PrecioPorKg { get; set; }

        public string ImagenUrl { get; set; } = "";

        public decimal ExistenciaKg { get; set; }

        public bool Disponible { get; set; }

        public bool PuedeAgregar { get; set; }

        public string Estado { get; set; } = "";
    }
}
