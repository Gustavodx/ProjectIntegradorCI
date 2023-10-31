using System.ComponentModel.DataAnnotations;

namespace ProyectoPI.Models
{
    public class DetalleVenta
    {
        [Key]
        public int Id { get; set; }
        public int VentaId { get; set; }
        public int MakiId { get; set; }
        public int Cantidad { get; set; }
        public int CompradorId { get; set; }
    }
}
