using Microsoft.EntityFrameworkCore;
using InsureFlow.PropostaService.Domain.Entities;

namespace InsureFlow.PropostaService.Infrastructure.Persistence
{
    public class PropostaDbContext : DbContext
    {
        public PropostaDbContext(DbContextOptions<PropostaDbContext> options) : base(options) { }

        public DbSet<Proposta> Propostas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Proposta>(b =>
            {
                b.ToTable("propostas");
                b.HasKey(x => x.Id);
                b.Property(x => x.NomeSegurado).IsRequired();
                b.Property(x => x.TipoSeguro).IsRequired();
                b.Property(x => x.ValorCobertura).HasColumnType("numeric");
                b.Property(x => x.PremioMensal).HasColumnType("numeric");
                b.Property(x => x.Status).HasConversion<int>();
                b.Property(x => x.DataCriacao);
                b.Property(x => x.DataAtualizacao);
            });
        }
    }
}
