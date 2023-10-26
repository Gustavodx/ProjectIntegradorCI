using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoPI.Models;

namespace ProyectoPI.Controllers
{
    public class CarritoController : Controller
    {
        private readonly DBPRUEBAContext _context;

        public CarritoController(DBPRUEBAContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var carrito = HttpContext.Session.Get<Carrito>("Carrito");
            return View(carrito);
        }

    }
}
