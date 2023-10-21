using Microsoft.EntityFrameworkCore;
using ProyectoPI.Models;

namespace ProyectoPI.Servicios.Contrato
{
    public interface IUsuarioService
    {
        Task<Usuario> GetUsuario(string correo, string clave);

        Task<Usuario> SaveUsuario(Usuario modelo);
    }
}
