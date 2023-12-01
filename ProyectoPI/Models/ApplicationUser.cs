using Microsoft.AspNetCore.Identity;

namespace ProyectoPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
