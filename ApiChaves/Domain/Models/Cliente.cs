using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiChaves.Domain.Models
{
    public class Cliente
    {
        [Key]
        public int ClienteID { get; set; }

        [Required]
 
        public string? ClienteCNPJ { get; set; }

        [Required]
        public string? ClienteNomeRazaoSocial { get; set; }

        [Required]
        public string? ClienteNomeFantasia { get; set; }

        [Required]
        public bool ClienteStatus { get; set; }

        public ICollection<ClienteProdutoChave> ClienteProdutoChaves { get; set; }
    }
}
