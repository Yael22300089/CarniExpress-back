v
namespace CarniExpress_back.Models
{
    public class CarritoItem
    {
        public int ProductoId { get; set; }

        public string Nombre { get; set; } = "";

        public decimal CantidadKg { get; set; }

        public decimal PrecioPorKg { get; set; }

        public decimal Subtotal =>
            CantidadKg * PrecioPorKg;
    }
}
