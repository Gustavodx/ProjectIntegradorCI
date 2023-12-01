using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoPI.Models;
using System.Security.Claims;
using static ProyectoPI.Models.Carrito;

namespace ProyectoPI.Controllers
{
    public class MakisController : Controller
    {
        private readonly DBPRUEBAContext _context;
        private readonly CarritoDeCompras _carrito;

        public MakisController(DBPRUEBAContext context, CarritoDeCompras carrito)
        {
            _context = context;
            _carrito = carrito;
        }

        public IActionResult Index()
        {
            var makis = _context.Makis.ToList();
            ViewData["Carrito"] = _carrito.Items;
            return View(makis);
        }

        // POST: Makis/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre, Ingredientes, Precio")] Maki maki)
        {
            if (ModelState.IsValid)
            {
                _context.Add(maki);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(maki);
        }

        [HttpPost]
        public IActionResult AddToCart(int makiId)
        {
            var maki = _context.Makis.Find(makiId);
            if (maki != null)
            {
                // Verificar si el maki ya está en el carrito y, si es así, incrementar la cantidad.
                var carritoItem = _carrito.Items.Find(item => item.Maki.Id == maki.Id);
                if (carritoItem != null)
                {
                    carritoItem.Cantidad++;
                }
                else
                {
                    _carrito.Items.Add(new CarritoItem { Maki = maki, Cantidad = 1 });
                }
            }

            return Json(new { success = true, message = "Producto agregado al carrito" });
        }

        [HttpPost]
        public IActionResult ConfirmarCompra([FromBody] Venta carrito)
        {
            // Obtener el ID de usuario de la sesión (reemplaza esto con tu lógica real de obtener el ID del usuario)
            var nombreUsuario = User.Identity.Name;
            var usuarioId = _context.Usuarios.First(u => u.Nombre == nombreUsuario).IdUsuario;
            int nuevoId = _context.Ventas.Max(v => (int?)v.Id) ?? 0;
            if (string.IsNullOrWhiteSpace(carrito.CodigoVenta))
            {
                carrito.CodigoVenta = $"VEN{usuarioId}{new Random().Next(1000, 9999)}000";
            }
            nuevoId++;
            // Crear una nueva venta
            var venta = new Venta
            {
                Id = nuevoId,
                UsuarioId = usuarioId,
                CodigoVenta = carrito.CodigoVenta,
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

            return Json("Compra Satisfacotria");
        }


    }
}
