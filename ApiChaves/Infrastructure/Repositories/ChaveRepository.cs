using ApiChaves.Application.DTOS;
using ApiChaves.Domain.Models;
using ApiChaves.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiChaves.Infrastructure.Repositories
{
    public class ChaveRepository
    {
        private readonly APIDbContext _context;

        public ChaveRepository(APIDbContext context)
        {
            _context = context;
        }
        public bool AddChave(Chave chave, ClienteProdutoChave clienteProdutoChave)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Chave.Add(chave);
                    _context.SaveChanges();

                    clienteProdutoChave.IdChave = chave.ChaveID;
                    var result = AddClienteProdutoChave(clienteProdutoChave);

                    if (result)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    return result;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Console.WriteLine(e); // Exibe a exceção completa
                    return false;
                }
            }
        }

        public bool AddClienteProdutoChave(ClienteProdutoChave chave)
        {
            try
            {
                _context.ClienteProdutoChave.Add(chave);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public Chave GetChave(string licensa, string mac, string cnpj, int produtoId)
        {
            var chave = (from ch in _context.Chave
                         join cp in _context.chaveProdutos on ch.ChaveID equals cp.IdChave
                         join p in _context.Produto on cp.IdProduto equals p.ProdutoID
                         join cc in _context.clienteChaves on ch.ChaveID equals cc.IdChave
                         join cl in _context.Cliente on cc.IdCliente equals cl.ClienteID
                         where EF.Functions.Like(ch.ChaveLicenca.Trim(), licensa.Trim())
                            && ch.ChaveMac.Trim() == mac.Trim()
                            && cl.ClienteCNPJ.Trim() == cnpj.Trim()
                            && p.ProdutoID == produtoId
                            && p.ProdutoStatus == true // ou == 1 se for int
                         select ch).FirstOrDefault();

            return chave;
        }

        public async Task<List<ChaveDto>> GetAllChaves()
        {
            // Consulta que une as tabelas usando Include e ThenInclude para incluir as relações
            var chaves = await _context.ClienteProdutoChave
                .Include(cpc => cpc.Cliente)
                .Include(cpc => cpc.Produto)
                .Include(cpc => cpc.Chave)
                .Select(cpc => new ChaveDto
                {
                    IdProduto = cpc.IdProduto,
                    CNPJCliente = cpc.Cliente.ClienteCNPJ,
                    Mac = cpc.Chave.ChaveMac, // Supondo que o Mac está armazenado aqui (ajuste se necessário)
                    Licenca = cpc.Chave.ChaveLicenca,
                    Status = cpc.Chave.ChaveStatus
                })
                .ToListAsync();

            return chaves;
        }


        public bool RenovarLicenca(int chaveId)
        {
            var chave = _context.Chave.FirstOrDefault(c => c.ChaveID == chaveId);
            if (chave != null)
            {
                chave.ChaveDataExpiracao = chave.ChaveDataExpiracao.AddMonths(1); // Adiciona 1 mês
                _context.Chave.Update(chave);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        // Método para bloquear a licença (alterar status para false)
        public bool BloquearLicenca(int chaveId)
        {
            var chave = _context.Chave.FirstOrDefault(c => c.ChaveID == chaveId);
            if (chave != null)
            {
                chave.ChaveStatus = false; // Define o status como false (bloqueado)
                _context.Chave.Update(chave);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
