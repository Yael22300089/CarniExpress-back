
namespace CarniExpress_back.Models
{
    public class Producto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = "";

        public string Categoria { get; set; } = "";

        public decimal PrecioPorKg { get; set; }

        public string ImagenUrl { get; set; } = "";

        public decimal ExistenciaKg { get; set; }

        public bool Activo { get; set; }

        public string Descripcion { get; set; } = "";
    }
}
