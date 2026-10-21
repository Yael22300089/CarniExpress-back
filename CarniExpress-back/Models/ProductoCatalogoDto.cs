
namespace CarniExpress_back.Models
{
    public class ProductoCatalogoDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = "";

        public string Categoria { get; set; } = "";

        public decimal PrecioPorKg { get; set; }

        public string ImagenUrl { get; set; } = "";

        public bool Disponible { get; set; }

        public bool PuedeAgregar { get; set; }
    }
}
