using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiChaves.Domain.Models
{
    [Table("ClienteProdutoChave")]
    public class ClienteProdutoChave
    {
        [Key]
        public int ClienteProdutoChaveID { get; set; }
        [Required]
        public string? CnpjCliente { get; set; }
        [Required]
        [ForeignKey("Produto")]
        public int IdProduto { get; set; }
        [Required]
        [ForeignKey("Chave")]
        public int IdChave { get; set; }
        public Cliente? Cliente { get; set; }
        public Produto? Produto { get; set; }
        public Chave? Chave { get; set; }

    }
}
