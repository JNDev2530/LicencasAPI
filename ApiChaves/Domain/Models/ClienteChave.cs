using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiChaves.Domain.Models
{
    [Table("clienteChave")]
    public class ClienteChave
    {
        [Key]
        public int ClienteChaveID { get; set; }
        [Required]
        public int IdCliente { get; set; }
        [Required]
        public int IdChave { get; set; }
       
    }
}
