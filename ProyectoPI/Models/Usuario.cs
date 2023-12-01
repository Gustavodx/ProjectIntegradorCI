using System;
using System.Collections.Generic;
using static ProyectoPI.Models.Carrito;

namespace ProyectoPI.Models
{
    public partial class Usuario
    {
        public int IdUsuario { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string? Clave { get; set; }
    }
}
