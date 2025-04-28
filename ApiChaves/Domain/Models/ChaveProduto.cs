using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiChaves.Domain.Models
{
    [Table("chaveProduto")]
    public class ChaveProduto
    {
        [Key]
        public int ChaveProdutoID { get; set; }
        [Required]
        public int IdChave { get; set; }
        [Required]
        public int IdProduto { get; set; }
    }
}
