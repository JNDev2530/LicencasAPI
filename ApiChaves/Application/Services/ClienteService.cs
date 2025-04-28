using ApiChaves.Application.DTOS;
using ApiChaves.Domain.Models;
using ApiChaves.Infrastructure.Repositories;

namespace ApiChaves.Application.Services
{
    public class ClienteService
    {
        private readonly ClienteRepository _clienteRepository;

        public ClienteService(ClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public bool ValidateCliente(string nome)
        {
            return !string.IsNullOrWhiteSpace(nome) && nome.Length >= 3;
        }

        public bool addCliente(ClienteDto cliente)
        {
            if (!ValidateCliente(cliente.RazaoSocial))
                throw new ArgumentException("O nome do produto deve ter pelo menos 3 caracteres.");

            // Obtém o produto do banco de dados
            Cliente clienteDB = _clienteRepository.GetCliente(cliente.RazaoSocial);
      

            // Verifica se produtoDB é nulo
            if (clienteDB == null)
            {
                Cliente novoCliente = new Cliente()
                {
                    ClienteCNPJ = cliente.CNPJ,
                    ClienteNomeRazaoSocial = cliente.RazaoSocial,  // Corrige para usar produto.Nome, pois produtoDB é nulo
                    ClienteNomeFantasia = cliente.NomeFantasia,
                    ClienteStatus = cliente.Status,
                    // Supondo que ProdutoStatus também esteja no ProdutoDto
                };

                return _clienteRepository.AddCliente(novoCliente);
            }

            // Se o produto já existe, retorna falso para indicar que ele não foi adicionado
            return false;
        }

        public List<ClienteDto> GetAllClientes()
        {
            var clientes = _clienteRepository.GetAllClientes(); // Chama o repositório para obter todos os produtos
            return clientes.Select(c => new ClienteDto
            {
                RazaoSocial = c.ClienteNomeRazaoSocial,
                CNPJ = c.ClienteCNPJ,
                NomeFantasia = c.ClienteNomeFantasia,
                Status = c.ClienteStatus,
                // Mapeie outras propriedades, se necessário
            }).ToList();
        }
    }
}
