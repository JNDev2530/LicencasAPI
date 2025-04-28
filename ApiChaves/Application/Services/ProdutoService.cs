using ApiChaves.Application.DTOS;
using ApiChaves.Domain.Models;
using ApiChaves.Infrastructure.Repositories;

namespace ApiChaves.Application.Services
{
    public class ProdutoService
    {
        private readonly ProdutoRepository _produtoRepository;

        public ProdutoService(ProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public bool ValidateProduto(string nome)
        {
            return !string.IsNullOrWhiteSpace(nome) && nome.Length >= 3;
        }

        public bool addProduto(ProdutoDto produto)
        {
            if (!ValidateProduto(produto.Nome))
                throw new ArgumentException("O nome do produto deve ter pelo menos 3 caracteres.");

            // Obtém o produto do banco de dados
            Produto produtoDB = _produtoRepository.GetProdutoNome(produto.Nome);

            // Verifica se produtoDB é nulo
            if (produtoDB == null)
            {
                Produto novoProduto = new Produto()
                {
                    ProdutoNome = produto.Nome,  // Corrige para usar produto.Nome, pois produtoDB é nulo
                    ProdutoStatus = produto.Status // Supondo que ProdutoStatus também esteja no ProdutoDto
                };

                return _produtoRepository.AddProduto(novoProduto);
            }

            // Se o produto já existe, retorna falso para indicar que ele não foi adicionado
            return false;
        }

        public List<ProdutoDto> GetAllProdutos()
        {
            var produtos = _produtoRepository.GetAllProdutos(); // Chama o repositório para obter todos os produtos
            return produtos.Select(p => new ProdutoDto
            {
                Nome = p.ProdutoNome,
                Status = p.ProdutoStatus
                // Mapeie outras propriedades, se necessário
            }).ToList();
        }
    }
}
