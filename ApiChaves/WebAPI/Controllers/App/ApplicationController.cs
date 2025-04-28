using ApiChaves.Application.DTOS;
using ApiChaves.Application.Services;
using ApiChaves.WebAPI.Controllers.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiChaves.WebAPI.Controllers.App
{

    [ApiController]
    [Route("api/application")]
    [RequiresSession]
    public class ApplicationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicacaoService _applicacaoService;

        public ApplicationController(IConfiguration configuration, ApplicacaoService applicacaoService)
        {
            _configuration = configuration;
            _applicacaoService = applicacaoService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AppAuthRequestDto authRequest)
        {
            // Validação inicial do corpo da requisição
            if (authRequest == null)
            {
                return BadRequest("A requisição está vazia.");
            }

            if (string.IsNullOrEmpty(authRequest.AppName))
            {
                return BadRequest("O campo AppName é obrigatório.");
            }

            if (string.IsNullOrEmpty(authRequest.AppSecret))
            {
                return BadRequest("O campo AppSecret é obrigatório.");
            }

            // Busca a aplicação pelo nome
            var aplicacao = await _applicacaoService.GetAplicacaoByNameAsync(authRequest.AppName);

            if (aplicacao == null)
            {

                return Unauthorized("Aplicação não encontrada.");
            }

            // Verifica se a senha é válida
            var isPasswordValid = _applicacaoService.VerifyPasswordHash(authRequest.AppSecret, aplicacao.ApplicacaoSenha);

            if (!isPasswordValid)
            {
                return Unauthorized("A senha fornecida está incorreta.");
            }

            // Gera o token
            var token = _applicacaoService.GenerateJwtToken(aplicacao);

            if (string.IsNullOrEmpty(token))
            {
                return StatusCode(500, "Erro ao gerar o token. Tente novamente mais tarde.");
            }

            return Ok(new { Token = token });
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterApplication([FromBody] AppAuthRequestDto aplicacaoDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(aplicacaoDto.AppName) || string.IsNullOrWhiteSpace(aplicacaoDto.AppSecret))
                {
                    return BadRequest("Nome e senha da aplicação são obrigatórios.");
                }

                // Aguarda o resultado do método assíncrono
                var resultado = await _applicacaoService.CadastrarAplicacaoAsync(aplicacaoDto.AppName, aplicacaoDto.AppSecret);

                if (resultado)
                {
                    return Ok(new { Message = "Aplicação cadastrada com sucesso!" });
                }

                return BadRequest("Erro ao cadastrar a aplicação.");
            }
            catch (Exception ex)
            {
                // Retorna uma resposta genérica em caso de erro
                return StatusCode(500, $"Erro interno ao cadastrar a aplicação: {ex.Message}");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("protected-resource")]
        public IActionResult GetProtectedResource()
        {
            // Este endpoint requer JWT e é acessível apenas por aplicativos autorizados
            return Ok("Conteúdo protegido acessado com sucesso.");
        }

        private async Task<bool> IsValidApplication(AppAuthRequestDto authRequest)
        {
            // Verifica se os dados do DTO são válidos
            if (authRequest == null || string.IsNullOrEmpty(authRequest.AppSecret))
            {
                return false;
            }

            // Converte o AppId para inteiro
            

            // Busca a aplicação pelo AppId no banco de dados
            var applicacao = await _applicacaoService.GetApplicacaoByIdAsync(authRequest.AppId);
            if (applicacao == null)
            {
                return false;
            }

            // Valida o AppSecret fornecido com o hash armazenado
            return VerifyAppSecret(authRequest.AppSecret, applicacao.ApplicacaoSenha);
        }


        private string GenerateJwtToken(string appId)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, appId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyAppSecret(string appSecret, string storedHash)
        {
            // Utiliza BCrypt para verificar se o segredo fornecido corresponde ao hash armazenado
            return BCrypt.Net.BCrypt.Verify(appSecret, storedHash);
        }
    }
}

