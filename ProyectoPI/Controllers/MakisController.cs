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


    }
}
