using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoPI.Models;
using static ProyectoPI.Models.Carrito;

namespace ProyectoPI.Controllers
{
    public class CarritoController : Controller
    {
        private readonly CarritoDeCompras _carrito;

        public CarritoController(CarritoDeCompras carrito)
        {
            _carrito = carrito;
        }
        public IActionResult Index()
        {
            var items = _carrito.Items.ToList();
            return View(items);
        }

        [HttpPost]
        public IActionResult IncrementQuantity(int makiId)
        {
            // Busca el producto en el carrito
            var carritoItem = _carrito.Items.FirstOrDefault(item => item.Maki.Id == makiId);

            if (carritoItem != null)
            {
                // Incrementa la cantidad del producto en el carrito
                carritoItem.Cantidad++;
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DecrementQuantity(int makiId)
        {
            // Busca el producto en el carrito
            var carritoItem = _carrito.Items.FirstOrDefault(item => item.Maki.Id == makiId);

            if (carritoItem != null)
            {
                // Decrementa la cantidad del producto en el carrito
                carritoItem.Cantidad--;

                if (carritoItem.Cantidad <= 0)
                {
                    // Si la cantidad llega a cero o menos, elimina el producto del carrito
                    _carrito.Items.Remove(carritoItem);
                }
            }

            return Json(new { success = true });
        }

        public IActionResult UpdateCarrito()
        {
            var items = _carrito.Items.ToList();
            return PartialView("CarritoItems", items);
        }

    }
}
