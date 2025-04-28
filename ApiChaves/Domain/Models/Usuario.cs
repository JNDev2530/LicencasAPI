using System.ComponentModel.DataAnnotations;

namespace ApiChaves.Domain.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioID { get; set; }
        [Required]
        public string? UsuarioNome { get; set; }
        [Required]
        public string? UsuarioSenha { get; set; }
    }
}
