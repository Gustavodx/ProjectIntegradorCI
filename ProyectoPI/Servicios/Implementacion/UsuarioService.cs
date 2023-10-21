using Microsoft.EntityFrameworkCore;
using ProyectoPI.Models;
using ProyectoPI.Servicios.Contrato;

namespace ProyectoPI.Servicios.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly DBPRUEBAContext _dbContext;

        public UsuarioService(DBPRUEBAContext dbcontect)
        {
            _dbContext = dbcontect;
        }
        public async Task<Usuario> GetUsuario(string correo, string clave)
        {
            Usuario usuario_encontrado = await _dbContext.Usuarios.Where(o => o.Correo == correo && o.Clave == clave)
                .FirstOrDefaultAsync();
            return usuario_encontrado;
        }

        public async Task<Usuario> SaveUsuario(Usuario modelo)
        {
            _dbContext.Usuarios.Add(modelo);
            await _dbContext.SaveChangesAsync();
            return modelo;
        }
    }
}
