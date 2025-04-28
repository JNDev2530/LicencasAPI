using ApiChaves.Domain.Models;
using ApiChaves.Infrastructure.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiChaves.Application.Services
{
    public class ApplicacaoService
    {
        private readonly ApplicacaoRepository _aplicacaoRepository;
        private readonly IConfiguration _configuration;

        public ApplicacaoService(ApplicacaoRepository aplicacaoRepository, IConfiguration configuration)
        {
            _aplicacaoRepository = aplicacaoRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Método para registrar uma nova aplicação.
        /// </summary>
        public async Task<bool> CadastrarAplicacaoAsync(string appName, string appSecret)
        {
            try
            {
                // Gerar o hash da senha com BCrypt
                var passwordHash = GerarHashSenhaComBcrypt(appSecret);

                // Criar a aplicação
                var aplicacao = new Applicacao
                {
                    ApplicacaoNome = appName,
                    ApplicacaoSenha = passwordHash, // Armazena diretamente o hash
                    DataCriacao = DateTime.UtcNow,
                    DataAtualizacao = DateTime.UtcNow
                };

                // Salvar no repositório
                var resultado = await _aplicacaoRepository.AddApplicacaoAsync(aplicacao);

                if (!resultado)
                    return false;

                // Gerar o token após o registro
                var token = GenerateJwtToken(aplicacao);

                // Atualizar o token no banco de dados
                aplicacao.ApplicacaoToken = token;
                aplicacao.DataAtualizacao = DateTime.UtcNow;

                return await _aplicacaoRepository.UpdateApplicacaoAsync(aplicacao);
            }
            catch
            {
                // Log de erro (recomenda-se implementar)
                return false;
            }
        }

        /// <summary>
        /// Gera o hash de uma senha utilizando BCrypt.
        /// </summary>
        private string GerarHashSenhaComBcrypt(string senha)
        {
            // Gera o hash utilizando BCrypt
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }

        /// <summary>
        /// Método para verificar o hash da senha.
        /// </summary>
        public bool VerifyPasswordHash(string senha, string hashArmazenado)
        {
            // Verifica se a senha fornecida corresponde ao hash armazenado usando BCrypt
            return BCrypt.Net.BCrypt.Verify(senha, hashArmazenado);
        }

        /// <summary>
        /// Método para autenticar uma aplicação usando seu nome e senha.
        /// </summary>
        public async Task<string?> AuthenticateAsync(string nome, string senha)
        {
            var aplicacao = await _aplicacaoRepository.GetApplicacaoByNameAsync(nome);

            if (aplicacao == null || !VerifyPasswordHash(senha, aplicacao.ApplicacaoSenha))
                return null; // Autenticação falhou

            // Gera o token JWT
            var token = GenerateJwtToken(aplicacao);
            aplicacao.ApplicacaoToken = token;
            aplicacao.TokenExpiracao = DateTime.UtcNow.AddHours(1); // Expiração do token

            await _aplicacaoRepository.UpdateTokenAsync(aplicacao.ApplicacaoID, token, aplicacao.TokenExpiracao);

            return token;
        }

        /// <summary>
        /// Método para gerar um token JWT para uma aplicação.
        /// </summary>
        public string GenerateJwtToken(Applicacao aplicacao)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, aplicacao.ApplicacaoNome),
                    new Claim("AppID", aplicacao.ApplicacaoID.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Tempo de expiração do token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Método para atualizar os detalhes da aplicação.
        /// </summary>
        public async Task<bool> UpdateApplicationAsync(int aplicacaoID, string nome, string? senha = null)
        {
            var aplicacao = await _aplicacaoRepository.GetApplicacaoByIdAsync(aplicacaoID);
            if (aplicacao == null) return false;

            aplicacao.ApplicacaoNome = nome;

            // Atualiza a senha se uma nova senha for fornecida
            if (!string.IsNullOrEmpty(senha))
            {
                var passwordHash = GerarHashSenhaComBcrypt(senha);
                aplicacao.ApplicacaoSenha = passwordHash;
            }

            aplicacao.DataAtualizacao = DateTime.UtcNow;
            return await _aplicacaoRepository.UpdateApplicacaoAsync(aplicacao);
        }

        /// <summary>
        /// Método para obter uma aplicação pelo seu nome.
        /// </summary>
        public async Task<Applicacao?> GetAplicacaoByNameAsync(string nome)
        {
            return await _aplicacaoRepository.GetApplicacaoByNameAsync(nome);
        }

        /// <summary>
        /// Método para obter uma aplicação pelo seu ID.
        /// </summary>
        public async Task<Applicacao?> GetApplicacaoByIdAsync(int appId)
        {
            return await _aplicacaoRepository.GetByIdAsync(appId);
        }
    }
}
