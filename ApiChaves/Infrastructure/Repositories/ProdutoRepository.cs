using ApiChaves.Domain.Models;
using ApiChaves.Infrastructure.Data.Context;

namespace ApiChaves.Infrastructure.Repositories
{
    public class ProdutoRepository
    {
        private readonly APIDbContext _context;

        public ProdutoRepository(APIDbContext context)
        {
            _context = context;
        }

        public bool AddProduto(Produto produto)
        {
            try
            {
                _context.Produto.Add(produto);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public Produto GetProduto(int idProduto)
        {
            return _context.Produto.FirstOrDefault(p => p.ProdutoID  == idProduto);
        }

        public Produto GetProdutoNome(string nome)
        {
            return _context.Produto.FirstOrDefault(p => p.ProdutoNome == nome);
        }

        // Método para obter todos os produtos
        public List<Produto> GetAllProdutos()
        {
            return _context.Produto.ToList();
        }
    }
}
