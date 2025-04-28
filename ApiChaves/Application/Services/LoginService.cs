using BCrypt.Net;
using ApiChaves.Application.DTOS;
using ApiChaves.Domain.Models;
using ApiChaves.Infrastructure.Repositories;

namespace ApiChaves.Application.Services
{
  
        public class LoginService
        {
            private readonly UsuarioRepository _usuarioRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public LoginService(UsuarioRepository usuarioRepository, IHttpContextAccessor httpContextAccessor)
            {
                _usuarioRepository = usuarioRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public UsuarioDto ValidateUser(string username, string password)
            {
                Usuario usuario = _usuarioRepository.GetUsuario(username);

                if (usuario != null && BCrypt.Net.BCrypt.Verify(password, usuario.UsuarioSenha))
                {
                    // Definir informações na sessão após o login
                    UsuarioDto user = new UsuarioDto()
                    {
                        Id = usuario.UsuarioID,
                        Nome = usuario.UsuarioNome
                    };
                    return user; // Usuário válido
                }

                return null; // Usuário inválido
            }

            public bool InsertUser(string username, string password)
            {
                // Verificar se o usuário já existe
                if (_usuarioRepository.GetUsuario(username) != null)
                {
                    return false; // Usuário já existe, não pode ser inserido novamente
                }

                // Criptografar a senha
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                // Criar um novo usuário
                Usuario novoUsuario = new Usuario
                {
                    UsuarioNome = username,
                    UsuarioSenha = hashedPassword
                    // Outros atributos do usuário, se houver
                };

                // Adicionar o usuário usando o repositório
                return _usuarioRepository.AddUsuario(novoUsuario);
            }

            public string GetHash(string password)
            {
                return BCrypt.Net.BCrypt.HashPassword(password);
            }
        
    }
}
