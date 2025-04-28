using ApiChaves.Application.DTOS;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Session;


namespace ApiChaves.Application.Services
{
    public class SessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public UsuarioDto GetUser()
        {
            var idString = this.GetUserId();
            var nome = this.GetUserName();

            if (idString != null && int.TryParse(idString, out var id))
            {
                return new UsuarioDto { Id = id, Nome = nome };
            }
            else
            {
                // Tratar o caso em que o ID não pôde ser convertido para int
                return null;
            }
        }

        private string GetUserId()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("Id");
        }

        private string GetUserName()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("Usuario");
        }

        public void SetUser(UsuarioDto user)
        {
            _httpContextAccessor.HttpContext.Session.SetString("Id", user.Id.ToString());
            _httpContextAccessor.HttpContext.Session.SetString("Usuario", user.Nome);
        }

        public void ClearSession()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
        }

        // Outros métodos de manipulação de sessão conforme necessário
    }
}
