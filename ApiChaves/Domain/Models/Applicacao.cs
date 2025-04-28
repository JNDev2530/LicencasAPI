using System.ComponentModel.DataAnnotations.Schema;

namespace ApiChaves.Domain.Models
{
    [Table("applicacao")]
    public class Applicacao
    {
        public int ApplicacaoID { get; set; }
        public string ApplicacaoNome { get; set; }
        public string ApplicacaoSenha { get; set; } // Armazenada como hash para segurança
        public string ApplicacaoToken { get; set; }
        public DateTime? TokenExpiracao { get; set; } // Data de expiração opcional
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}
