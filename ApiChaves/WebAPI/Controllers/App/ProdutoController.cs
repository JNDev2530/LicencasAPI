using ApiChaves.Application.DTOS;
using ApiChaves.Application.Services;
using ApiChaves.WebAPI.Controllers.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiChaves.WebAPI.Controllers.App
{
    [Route("api/[controller]")]
    [ApiController]
    [RequiresSession]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoService _produtoService;

        public ProdutoController(ProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

      
        [HttpGet]
        public IActionResult GetProduto()
        {
            var produtos = _produtoService.GetAllProdutos(); // Recupera todos os produtos através do serviço

            if (produtos == null || !produtos.Any())
            {
                return NotFound("Nenhum produto encontrado.");
            }

            return Ok(produtos); // Retorna a lista de produtos
        }

        [HttpPost("adicionar")]
        public IActionResult InsertProduto(ProdutoDto produtoDto)
        {
            try
            {
                var result = _produtoService.addProduto(produtoDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
