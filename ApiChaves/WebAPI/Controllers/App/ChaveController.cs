using ApiChaves.Application.DTOS;
using ApiChaves.Application.Services;
using ApiChaves.WebAPI.Controllers.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiChaves.WebAPI.Controllers.App
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ChaveController : ControllerBase
    {
        private readonly ChaveService _chaveService;

        // Injetando o serviço ChaveService
        public ChaveController(ChaveService chaveService)
        {
            _chaveService = chaveService;
        }
        
        // Endpoint para adicionar chave
        [HttpPost("adicionar")]
        [RequiresSession]
        public IActionResult AdicionarChave(ChaveDto chaveDto)
        {
            var sucesso = _chaveService.AddChave(HttpContext, chaveDto); // Passando HttpContext para o serviço

            if (sucesso)
            {
                return Ok("Chave gerada com sucesso.");
            }
            else
            {
                return BadRequest("Erro ao gerar chave.");
            }
        }

        [HttpGet("validar")]
        public async Task<IActionResult> ValidarLicenca([FromQuery] string licensa, [FromQuery] string mac, [FromQuery] string cnpj, [FromQuery] int produtoId)
        {
            bool isValid = await _chaveService.ValidarLicencaAsync(licensa, mac, cnpj, produtoId);
            return Ok(isValid);
        }

        
        // Endpoint para renovar a licença de uma chave (adicionando 1 mês à data de expiração)
        [HttpPost("renovar/{chaveId}")]
        [RequiresSession]
        public IActionResult RenovarLicenca(int chaveId)
        {
            var sucesso = _chaveService.RenovarLicenca(chaveId);

            if (sucesso)
            {
                return Ok("Licença renovada com sucesso.");
            }
            else
            {
                return BadRequest("Erro ao renovar a licença.");
            }
        }

       
        // Endpoint para bloquear a licença de uma chave (alterando o status para false)
        [HttpPost("bloquear/{chaveId}")]
        [RequiresSession]
        public IActionResult BloquearLicenca(int chaveId)
        {
            var sucesso = _chaveService.BloquearLicenca(chaveId);

            if (sucesso)
            {
                return Ok("Licença bloqueada com sucesso.");
            }
            else
            {
                return BadRequest("Erro ao bloquear a licença.");
            }
        }

       
        [HttpGet]
        [RequiresSession]
        public async Task<ActionResult<IEnumerable<ChaveDto>>> GetAllChaves()
        {
            var chaves = await _chaveService.GetAllChaves();
            return Ok(chaves);
        }
    }
}
