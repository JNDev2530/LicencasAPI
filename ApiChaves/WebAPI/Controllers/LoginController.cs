using ApiChaves.Application.DTOS;
using ApiChaves.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiChaves.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly LoginService _loginService;
        private readonly SessionService _sessionService;

        public LoginController(LoginService loginService, SessionService sessionService)
        {
            _loginService = loginService;
            _sessionService = sessionService;
        }

        [HttpPost("autenticar")]
        public IActionResult Authenticate([FromBody] UsuarioLoginDto model)
        {
            var username = model.Username;
            var password = model.Password;
            UsuarioDto usuariobd = _loginService.ValidateUser(username, password);

            if (usuariobd != null)
            {
                _sessionService.SetUser(usuariobd);
                var usuarioLogado = _sessionService.GetUser();
                return Ok(usuarioLogado);
            }
            else
            {
                return Unauthorized("Usuário ou senha incorretos.");
            }
        }

        [HttpPost("registrarUsuario")]
        public IActionResult Register([FromBody] UsuarioRegisterDto model)
        {
            var username = model.Username;
            var password = model.Password;

            if (_loginService.InsertUser(username, password))
            {
                return Ok("Usuário registrado com sucesso!");
            }
            else
            {
                return BadRequest("Usuário já existe.");
            }
        }
    }
}
