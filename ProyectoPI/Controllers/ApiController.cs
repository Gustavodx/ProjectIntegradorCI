using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoPI.Models;
using System.Web.Helpers;
using static ProyectoPI.Models.Carrito;

namespace ProyectoPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly DBPRUEBAContext _context;
        private readonly CarritoDeCompras _carrito;
        private readonly IEmailSender _emailSender;

        public ApiController(DBPRUEBAContext context, CarritoDeCompras carrito, IEmailSender emailSender)
        {
            _context = context;
            _carrito = carrito;
            _emailSender = emailSender;
        }


        [HttpGet]
        [Route("makis/GetMakis")]
        public ActionResult<IEnumerable<Maki>> GetMakis()
        {
            var makis = _context.Makis.ToList();
            return makis;
        }

        [HttpGet]
        [Route("GetVentaCodigos/{usuarioId}")]
        public ActionResult<IEnumerable<string>> GetVentaCodigos(int usuarioId)
        {
            // Filtrar las ventas por el usuarioId
            var ventas = _context.Ventas.Where(v => v.UsuarioId == usuarioId).ToList();

            // Obtener solo los códigos de venta
            var codigosVenta = ventas.Select(v => v.CodigoVenta).ToList();

            return codigosVenta;
        }

        [HttpGet("GetUltimaVentaPorId/{ventaId}")]
        public ActionResult<string> GetUltimaVentaPorId(int ventaId)
        {
            var ultimaVenta = _context.Ventas
                .OrderByDescending(v => v.Id)
                .FirstOrDefault();

            if (ultimaVenta == null)
            {
                return NotFound();
            }

            return ultimaVenta.CodigoVenta;
        }

        [HttpGet("GetUltimaVentaPorUsuario/{nombreUsuario}")]
        public ActionResult<string> GetUltimaVentaPorUsuario(string nombreUsuario)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Nombre == nombreUsuario);

            if (usuario == null)
            {
                return NotFound();
            }
            var ultimaVenta = _context.Ventas
                .Where(v => v.UsuarioId == usuario.IdUsuario)
                .OrderByDescending(v => v.Id)
                .FirstOrDefault();

            if (ultimaVenta == null)
            {
                return NotFound();
            }

            return ultimaVenta.CodigoVenta;
        }


        [HttpGet]
        [Route("GetVentaCodigosPorUsuario/{nombreUsuario}")]
        public ActionResult<IEnumerable<string>> GetVentaCodigosPorUsuario(string nombreUsuario)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Nombre == nombreUsuario);

            if (usuario == null)
            {
                return NotFound();
            }
            var ventas = _context.Ventas.Where(v => v.UsuarioId == usuario.IdUsuario).ToList();
            var codigosVenta = ventas.Select(v => v.CodigoVenta).ToList();

            return codigosVenta;
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

            return RedirectToAction("Index", "Makis");
        }

        /*********/

        [HttpPost]
        [Route("makis/ListarTodosMakis")]
        public ActionResult<IEnumerable<Maki>> ListarTodosMakis([FromBody] string dummy)
        {
            var makis = _context.Makis.ToList();
            return makis;
        }


        [HttpPost]
        [Route("consultas/NewConsulta")]
        public async Task<IActionResult> CrearConsulta([FromBody] Consultas consulta)
        {
            // Generar el código (CDA-000x)
            if(consulta.Correo == null || consulta.Correo == "")
            {
                var corre = User.Identity.Name;
                var correo = _context.Usuarios.First(u => u.Nombre == corre).Correo;
                consulta.Correo = correo;
            }
            int ultimaConsultaId = _context.Consultas.Max(c => (int?)c.Id) ?? 0;
            string codigo = $"CDA-{ultimaConsultaId + 1:0000}";
            consulta.Estado = 1;
            consulta.Codigo = codigo;

            await _emailSender.SendEmailAsync(consulta.Correo, "Hola Tu codigo", $"Hola me pongo en contacto se te genero el {consulta.Codigo}");
            _context.Consultas.Add(consulta);
            _context.SaveChanges();

            return Ok(new { Message = "Consulta creada con éxito", Codigo = codigo });
        }


        [HttpPost]
        [Route("consulta/NewSolicitud")]
        public async Task<IActionResult> CrearSolicitud([FromBody] CorreoModel correoModel)
        {
            try
            {
                if (string.IsNullOrEmpty(correoModel.Correo))
                {
                    return BadRequest("El correo no puede estar vacío");
                }
                string correo = correoModel.Correo;
                int ultimaConsultaId = _context.Mensajes.ToList().Max(c => (int?)c.Id) ?? 0;
                string codigo = $"CDAB-{ultimaConsultaId + 1:0000}";

                Mensajes mensajes = new Mensajes
                {
                    Correo = correo,
                    Codigo = codigo
                };

                await _emailSender.SendEmailAsync(correo, "Hola, tu código", $"Hola, me pongo en contacto. Se te generó el código: {mensajes.Codigo}");

                _context.Mensajes.Add(mensajes);
                _context.SaveChanges();

                return Ok(new { Message = $"Consulta creada con éxito {mensajes.Codigo}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Se produjo un error al guardar el correo: " + ex.Message);
            }
        }


        [HttpPost]
        [Route("consulta/NewSolicitudV2/{correo}")]
        public async Task<IActionResult> CrearSolicitudCorreo(string correo)
        {
            try
            {
                if (string.IsNullOrEmpty(correo))
                {
                    return BadRequest("El correo no puede estar vacio");
                }
                int ultimaConsultaId = _context.Mensajes.Max(c => (int?)c.Id) ?? 0;
                string codigo = $"CDAB2-{ultimaConsultaId + 1:0000}";
                Mensajes mensajes = new Mensajes
                {
                    Correo = correo,
                    Codigo = codigo
                };
                await _emailSender.SendEmailAsync(correo, "Hola Tu codigo", $"Hola me pongo en contacto se te genero el {mensajes.Codigo}");
                _context.Mensajes.Add(mensajes);
                _context.SaveChanges();
                return Ok(new { Message = "Consulta creada con éxito", Codigo = codigo });
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Se produjo un error al guardar el correo: " + ex.Message);
            }
        }
    }
}
