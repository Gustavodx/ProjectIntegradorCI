using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Security.Cryptography;
using System.Text;

namespace ProyectoPI.Recursos
{
    public class Utils
    {
        public static string EncriptarClave(string clave)
        {
            StringBuilder sb = new StringBuilder();
            using(SHA256 hash=SHA256Managed.Create())
            {
                Encoding ec = Encoding.UTF8;
                byte[] result = hash.ComputeHash(ec.GetBytes(clave));

                foreach (byte b in result)
                    sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
