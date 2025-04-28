using ApiChaves.Application.DTOS;
using ApiChaves.Application.Services;
using ApiChaves.WebAPI.Controllers.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiChaves.WebAPI.Controllers.App
{
    [Route("api/[controller]")]
    [ApiController]
    [RequiresSession]
    public class ClienteController : ControllerBase
    {
        private readonly ClienteService _clienteService;

        public ClienteController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public IActionResult GetCliente()
        {
            var clientes = _clienteService.GetAllClientes(); // Recupera todos os produtos através do serviço

            if (clientes == null || !clientes.Any())
            {
                return NotFound("Nenhum Cliente encontrado.");
            }

            return Ok(clientes); // Retorna a lista de produtos
        }

        [HttpPost("adicionar")]
        public IActionResult InsertClientes(ClienteDto clienteDto)
        {
            try
            {
                var result = _clienteService.addCliente(clienteDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
