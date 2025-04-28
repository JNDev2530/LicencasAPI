using System.ComponentModel.DataAnnotations;

namespace ApiChaves.Domain.Models
{
    public class Chave
    {
        [Key]
        public int ChaveID { get; set; }
        [Required]
        public string? ChaveMac { get; set; }
        [Required]
        public string? ChaveLicenca { get; set; }
        [Required]
        public DateTime ChaveDataCriacao { get; set; }
        [Required]
        public DateTime ChaveDataExpiracao { get; set; }
        [Required]
        public bool ChaveStatus { get; set; }
        public ICollection<ClienteProdutoChave> ClienteProdutoChaves { get; set; }
    }
}
