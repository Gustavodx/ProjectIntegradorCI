using Microsoft.AspNetCore.Mvc;
using ProyectoPI.Models;
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
        public IActionResult AgregarAlCarrito(int id, int cantidad)
        {
            var maki = _context.Makis.FirstOrDefault(m => m.Id == id);
            if (maki != null && cantidad > 0)
            {
                var carritoItem = new CarritoItem { Maki = maki, Cantidad = cantidad };
                _carrito.Items.Add(carritoItem);
            }
            return RedirectToAction("Index");
        }
    }
}
