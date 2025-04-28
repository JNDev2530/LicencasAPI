using System.ComponentModel.DataAnnotations;

namespace ApiChaves.Domain.Models
{
    public class Produto
    {
        [Key]
        public int ProdutoID { get; set; }
        [Required]
        public string? ProdutoNome { get; set; }
        [Required]
        public bool ProdutoStatus { get; set; }
        public ICollection<ClienteProdutoChave> ClienteProdutoChaves { get; set; }
    }
}
