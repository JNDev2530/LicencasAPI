using ApiChaves.Application.DTOS;
using ApiChaves.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiChaves.Infrastructure.Repositories;

namespace ApiChaves.Application.Services
{
    public class ChaveService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ChaveRepository _chaveRepository;
        private readonly ProdutoRepository _produtoRepository;
        private readonly ClienteRepository _clienteRepository;

        public ChaveService(IOptions<JwtSettings> jwtSettings, ChaveRepository chaveRepository,
            ProdutoRepository produtoRepository, ClienteRepository clienteRepository)
        {
            _jwtSettings = jwtSettings.Value;

            if (string.IsNullOrEmpty(_jwtSettings.Secret))
            {
                throw new ArgumentNullException(nameof(_jwtSettings.Secret), "O segredo do JWT não pode ser nulo ou vazio.");
            }

            _chaveRepository = chaveRepository;
            _produtoRepository = produtoRepository;
            _clienteRepository = clienteRepository;
        }

        public bool AddChave(HttpContext context, ChaveDto chavedto)
        {
            // Recuperando valores da sessão
            string id = context.Session.GetString("Id");
            string usuario = context.Session.GetString("Usuario");

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(usuario))
            {
                return false; // Caso não haja informações na sessão, não gera o token
            }

            // Gerando o token com base nas informações de id e usuário
            string licencagerada = GenerateToken(id, usuario);

            // Armazenando o token gerado na sessão
            Cliente cliente = _clienteRepository.GetCliente(chavedto.CNPJCliente);
            Produto produto = _produtoRepository.GetProduto(chavedto.IdProduto);
            Chave chave = new Chave()
            {
                ChaveLicenca = licencagerada,
                ChaveMac = chavedto.Mac,
                ChaveDataCriacao = DateTime.UtcNow,  // Data e hora atual
                ChaveDataExpiracao = DateTime.UtcNow.AddMonths(1),  // Um mês após a data de criação
                ChaveStatus = chavedto.Status
            };

            ClienteProdutoChave clienteProdutoChave = new ClienteProdutoChave() 
            { 
                CnpjCliente = cliente.ClienteCNPJ,
                IdProduto = produto.ProdutoID
            }; 
                

            return _chaveRepository.AddChave(chave, clienteProdutoChave);
        }

        public string GenerateToken(string usuarioId, string usuarioNome)
        {
            // Validação dos parâmetros para evitar valores nulos
            if (string.IsNullOrWhiteSpace(usuarioId))
                throw new ArgumentNullException(nameof(usuarioId), "O ID do usuário não pode ser nulo ou vazio.");

            if (string.IsNullOrWhiteSpace(usuarioNome))
                throw new ArgumentNullException(nameof(usuarioNome), "O nome do usuário não pode ser nulo ou vazio.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, usuarioId),
            new Claim(ClaimTypes.Name, usuarioNome)
        }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<List<ChaveDto>> GetAllChaves()
        {
            return await _chaveRepository.GetAllChaves();
        }

        public async Task<bool> ValidarLicencaAsync(string licensa, string mac, string cnpj, int produtoId)
        {
            // Como o GetChave é síncrono, podemos rodá-lo dentro de Task.Run ou refatorá-lo para ser assíncrono.
            var chave = await Task.Run(() => _chaveRepository .GetChave(licensa, mac, cnpj, produtoId));
            if (chave == null)
            {
                return false;
            }

            // Verifica se a data de expiração é válida e se o status está ativo (supondo que 1 seja ativo)
            if (chave.ChaveDataExpiracao > DateTime.Now && chave.ChaveStatus == true)
            {
                return true;
            }
            return false;
        }

        public bool RenovarLicenca(int chaveId)
        {
            return _chaveRepository.RenovarLicenca(chaveId);
        }

        // Método para bloquear a licença (alterar status para false)
        public bool BloquearLicenca(int chaveId)
        {
            return _chaveRepository.BloquearLicenca(chaveId);
        }


    }
}