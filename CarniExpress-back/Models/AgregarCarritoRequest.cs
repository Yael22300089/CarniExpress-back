
using System.ComponentModel.DataAnnotations;

namespace CarniExpress_back.Models
{
    public class AgregarCarritoRequest
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "El producto no es válido")]
        public int ProductoId { get; set; }

        [Range(typeof(decimal), "0.5", "999999",
            ErrorMessage = "La cantidad mínima es 0.5 kg")]
        public decimal CantidadKg { get; set; }
    }
}
