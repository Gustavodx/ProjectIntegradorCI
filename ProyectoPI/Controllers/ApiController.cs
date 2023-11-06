using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoPI.Models;
using static ProyectoPI.Models.Carrito;

namespace ProyectoPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly DBPRUEBAContext _context;
        private readonly CarritoDeCompras _carrito;

        public ApiController(DBPRUEBAContext context, CarritoDeCompras carrito)
        {
            _context = context;
            _carrito = carrito;
        }

        [HttpPost]
        [Route("ventas/confirmarcompra")]
        public IActionResult ConfirmarCompra([FromBody] Venta carrito)
        {
            // Obtener el ID de usuario de la sesión (reemplaza esto con tu lógica real de obtener el ID del usuario)
            var nombreUsuario = User.Identity.Name;
            var usuarioId = _context.Usuarios.First(u => u.Nombre == nombreUsuario).IdUsuario;
            int nuevoId = _context.Ventas.Max(v => (int?)v.Id) ?? 0;
            var codigoVenta = $"VEN{usuarioId}{new Random().Next(1000, 9999)}000";

            nuevoId++;
            // Crear una nueva venta
            var venta = new Venta
            {
                Id = nuevoId,
                UsuarioId = usuarioId,
                CodigoVenta = codigoVenta, // Implementa tu lógica para generar un código único
                MontoTotal = carrito.MontoTotal,
                DetalleVentas = new List<DetalleVenta>()
            };
            var idven = _context.DetalleVentas.Max(v => (int?)v.Id) ?? 0;
            idven++;
            var tun = idven;
            // Crear instancias de DetalleVenta para cada artículo en el carrito
            foreach (var item in carrito.DetalleVentas)
            {
                var detalleVenta = new DetalleVenta
                {
                    Id = tun,
                    VentaId = venta.Id,
                    MakiId = item.MakiId,
                    Cantidad = item.Cantidad,
                    CompradorId = usuarioId
                };

                tun++;
                venta.DetalleVentas.Add(detalleVenta);
            }

            _context.Ventas.Add(venta);
            _context.SaveChanges();

            _carrito.Items.Clear();

            return Ok("Compra Satisfacotria");
        }
    }
}
