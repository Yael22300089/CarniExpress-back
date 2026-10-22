
namespace CarniExpress_back.Models
{
    public class DesgloseProductoDto
    {
        public int ProductoId { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public decimal CantidadKg { get; set; }

        public decimal PrecioPorKg { get; set; }

        public decimal Subtotal { get; set; }
    }

    public class DesgloseCarritoResponse
    {
        public List<DesgloseProductoDto> Productos { get; set; }
            = new();

        public int TotalProductos { get; set; }

        public decimal TotalKg { get; set; }

        public decimal Total { get; set; }

        public bool CarritoVacio { get; set; }

        public string Mensaje { get; set; } = string.Empty;
    }
}
