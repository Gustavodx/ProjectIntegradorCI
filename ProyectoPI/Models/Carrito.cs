namespace ProyectoPI.Models
{
    public class Carrito
    {
        public class CarritoDeCompras
        {
            public List<CarritoItem> Items { get; } = new List<CarritoItem>();
        }

        public class CarritoItem
        {
            public Maki Maki { get; set; }
            public int Cantidad { get; set; }
        }
    }
}
