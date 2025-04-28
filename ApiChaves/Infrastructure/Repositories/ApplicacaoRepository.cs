using ApiChaves.Domain.Models;
using ApiChaves.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ApiChaves.Infrastructure.Repositories
{
    public class ApplicacaoRepository
    {
        private readonly APIDbContext _context;

        public ApplicacaoRepository(APIDbContext context)
        {
            _context = context;
        }

        // Método para adicionar uma nova aplicação
        public async Task<bool> AddApplicacaoAsync(Applicacao applicacao)
        {
            applicacao.DataCriacao = DateTime.UtcNow;
            applicacao.DataAtualizacao = DateTime.UtcNow;
            _context.Applicacoes.Add(applicacao);
            return await _context.SaveChangesAsync() > 0;
        }

        // Método para buscar uma aplicação por ID
        public async Task<Applicacao> GetApplicacaoByIdAsync(int applicacaoID)
        {
            return await _context.Applicacoes.FindAsync(applicacaoID);
        }

        // Método para buscar uma aplicação por nome
        public async Task<Applicacao> GetApplicacaoByNameAsync(string applicacaoNome)
        {
            return await _context.Applicacoes
                .FirstOrDefaultAsync(app => app.ApplicacaoNome == applicacaoNome);
        }

        

        // Método para atualizar o token e a data de expiração
        public async Task<bool> UpdateTokenAsync(int applicacaoID, string token, DateTime? tokenExpiracao)
        {
            var applicacao = await _context.Applicacoes.FindAsync(applicacaoID);
            if (applicacao == null) return false;

            applicacao.ApplicacaoToken = token;
            applicacao.TokenExpiracao = tokenExpiracao;
            applicacao.DataAtualizacao = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }

        // Método para atualizar uma aplicação (sem modificar a senha)
        public async Task<bool> UpdateApplicacaoAsync(Applicacao applicacao)
        {
            applicacao.DataAtualizacao = DateTime.UtcNow;
            _context.Applicacoes.Update(applicacao);
            return await _context.SaveChangesAsync() > 0;
        }

        // Método para deletar uma aplicação
        public async Task<bool> DeleteApplicacaoAsync(int applicacaoID)
        {
            var applicacao = await _context.Applicacoes.FindAsync(applicacaoID);
            if (applicacao == null) return false;

            _context.Applicacoes.Remove(applicacao);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Applicacao?> GetByIdAsync(int appId)
        {
            return await _context.Applicacoes
                .FirstOrDefaultAsync(a => a.ApplicacaoID == appId);
        }

       

    }
}
