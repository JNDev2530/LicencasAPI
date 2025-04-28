using System.ComponentModel.DataAnnotations;

namespace ApiChaves.Application.DTOS
{
    /// <summary>
    /// DTO usado para autenticação e registro de aplicações.
    /// </summary>
    public class AppAuthRequestDto
    {
        /// <summary>
        /// Nome da aplicação (obrigatório para registro).
        /// </summary>
        [Required(ErrorMessage = "O nome da aplicação é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da aplicação deve ter no máximo 100 caracteres.")]
        public string AppName { get; set; }

        /// <summary>
        /// Chave secreta da aplicação (obrigatório para registro e autenticação).
        /// </summary>
        [Required(ErrorMessage = "A chave secreta é obrigatória.")]
        [StringLength(255, ErrorMessage = "A chave secreta deve ter no máximo 255 caracteres.")]
        public string AppSecret { get; set; }

        /// <summary>
        /// Identificador da aplicação (necessário para autenticação de aplicações existentes).
        /// </summary>
        public int AppId { get; set; }
    }
}
