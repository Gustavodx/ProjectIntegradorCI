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
            // Lógica para agregar el maki al carrito.
            // Aquí debes buscar el maki por ID y agregarlo al carrito.

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
        public IActionResult ConfirmarCompra1([FromBody] Venta venta)
        {
            try
            {
                var nombreUsuario = User.Identity.Name;
                var usuarioId = _context.Usuarios.First(u => u.Nombre == nombreUsuario).IdUsuario;
                int nuevoId = _context.Ventas.Max(v => (int?)v.Id) ?? 0;
                nuevoId++;
                venta.UsuarioId = usuarioId;
                venta.CodigoVenta=$"VEN{usuarioId}{new Random().Next(1000, 9999)}000";
                venta.Id = nuevoId;
                // Aquí puedes guardar la venta y sus detalles en tu base de datos utilizando Entity Framework o tu mecanismo preferido
                _context.Ventas.Add(venta);

                var detallesVenta = new List<DetalleVenta>();

                foreach (var detalle in venta.DetalleVentas)
                {
                    var idven = _context.DetalleVentas.Max(v => (int?)v.Id) ?? 0;
                    detalle.Id = idven+1;
                    detalle.VentaId = venta.Id;
                    detalle.CompradorId = usuarioId;
                    _context.DetalleVentas.Add(detalle);
                }
                _context.DetalleVentas.AddRange(detallesVenta);
                _context.SaveChanges();

                return Json(new { message = "Compra confirmada exitosamente" });
            }
            catch (Exception ex)
            {
                // Maneja cualquier error que pueda ocurrir al guardar en la base de datos
                return Json(new { error = "Ocurrió un error al confirmar la compra." });
            }
        }

        [HttpPost]
        public IActionResult ConfirmarCompra([FromBody] Venta carrito)
        {
            // Obtener el ID de usuario de la sesión (reemplaza esto con tu lógica real de obtener el ID del usuario)
            var nombreUsuario = User.Identity.Name;
            var usuarioId = _context.Usuarios.First(u => u.Nombre == nombreUsuario).IdUsuario;
            int nuevoId = _context.Ventas.Max(v => (int?)v.Id) ?? 0;
            nuevoId++;
            // Crear una nueva venta
            var venta = new Venta
            {
                Id= nuevoId,
                UsuarioId = usuarioId,
                CodigoVenta = $"VEN{usuarioId}{new Random().Next(1000, 9999)}000", // Implementa tu lógica para generar un código único
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
