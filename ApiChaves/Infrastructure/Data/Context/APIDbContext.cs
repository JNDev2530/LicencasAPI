using ApiChaves.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiChaves.Infrastructure.Data.Context
{
    public class APIDbContext : DbContext
    {

        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
               
                 optionsBuilder.UseSqlServer(@"Server=.\SQL2019;Database=ReservemeChaves;User Id=sa;Password=c0mmercy;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.ClienteCNPJ)
                .IsUnique(); // Define ClienteCNPJ como um índice único

            modelBuilder.Entity<ClienteProdutoChave>()
                .HasOne(cp => cp.Cliente)
                .WithMany(c => c.ClienteProdutoChaves)
                .HasForeignKey(cp => cp.CnpjCliente)
                .HasPrincipalKey(c => c.ClienteCNPJ); // Define ClienteCNPJ como chave alternativa

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Produto> Produto { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Chave> Chave { get; set; }
        public DbSet<ClienteProdutoChave> ClienteProdutoChave { get; set; }
        public DbSet<Applicacao> Applicacoes { get; set; }
        public DbSet<ChaveProduto> chaveProdutos { get; set; }
        public DbSet<ClienteChave> clienteChaves { get; set; }
    }
}
