using System.ComponentModel.DataAnnotations;

namespace ProyectoPI.Models
{
    public class Maki
    {
        [Key]
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Ingredientes { get; set; }
        public decimal Precio { get; set; }

        public int Cantidad { get; set; }
    }
}
