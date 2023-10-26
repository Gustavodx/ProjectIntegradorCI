using Microsoft.AspNetCore.Mvc;
using ProyectoPI.Models;
using ProyectoPI.Recursos;
using ProyectoPI.Servicios.Contrato;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Data.SqlTypes;

namespace ProyectoPI.Controllers
{
    public class InicioController : Controller
    {
        private IUsuarioService _usuarioService;

        public InicioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>Registrarse(Usuario modelo)
        {
            modelo.Clave = Recursos.Utils.EncriptarClave(modelo.Clave);
            Usuario usuario_cra = await _usuarioService.SaveUsuario(modelo);
            if (usuario_cra.IdUsuario > 0)
                return RedirectToAction("IniciarSession", "Inicio");
            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
        }


        public IActionResult IniciarSession()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSession(string correo, string clave)
        {
            Usuario usuario_encontrado = await _usuarioService.GetUsuario(correo, Recursos.Utils.EncriptarClave(clave));
            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, usuario_encontrado.Nombre)
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties);
            return RedirectToAction("Index", "Home");
        }
    }
}
