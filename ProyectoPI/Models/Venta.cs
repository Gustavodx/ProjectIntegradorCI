using System.ComponentModel.DataAnnotations;

namespace ProyectoPI.Models
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string CodigoVenta { get; set; }
        public decimal MontoTotal { get; set; }
        public List<DetalleVenta> DetalleVentas { get; set; }
    }
}
