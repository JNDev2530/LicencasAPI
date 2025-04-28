using ApiChaves.Domain.Models;
using ApiChaves.Infrastructure.Data.Context;

namespace ApiChaves.Infrastructure.Repositories
{
    

    public class UsuarioRepository
    {
        private readonly APIDbContext _context;

        public UsuarioRepository(APIDbContext context)
        {
            _context = context;
        }

        public bool AddUsuario(Usuario usuario)
        {
            try
            {
                _context.Usuario.Add(usuario);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public Usuario GetUsuario(string username)
        {
            return _context.Usuario.FirstOrDefault(u => u.UsuarioNome == username);
        }
    }
}
