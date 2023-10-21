using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoPI.Controllers
{
    public class VistasController : Controller
    {
        public IActionResult Main()
        {
            return View();
        }

        public IActionResult Preguntas()
        {
            return View();
        }

        public IActionResult Nosotros()
        {
            return View();
        }


    }

}
