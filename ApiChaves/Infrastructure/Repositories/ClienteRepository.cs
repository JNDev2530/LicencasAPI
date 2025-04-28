using ApiChaves.Domain.Models;
using ApiChaves.Infrastructure.Data.Context;

namespace ApiChaves.Infrastructure.Repositories
{
    public class ClienteRepository
    {
        private readonly APIDbContext _context;

        public ClienteRepository(APIDbContext context)
        {
            _context = context;
        }

        public bool AddCliente(Cliente cliente)
        {
            try
            {
                _context.Cliente.Add(cliente);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public Cliente GetCliente(string cnpj)
        {
            return _context.Cliente.FirstOrDefault(c => c.ClienteCNPJ == cnpj);
        }

        // Método para obter todos os produtos
        public List<Cliente> GetAllClientes()
        {
            return _context.Cliente.ToList();
        }
    }
}
