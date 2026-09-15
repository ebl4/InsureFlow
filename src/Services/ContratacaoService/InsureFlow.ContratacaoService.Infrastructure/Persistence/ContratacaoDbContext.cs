using Microsoft.EntityFrameworkCore;
using InsureFlow.ContratacaoService.Domain.Entities;

namespace InsureFlow.ContratacaoService.Infrastructure.Persistence
{
    public class ContratacaoDbContext : DbContext
    {
        public ContratacaoDbContext(DbContextOptions<ContratacaoDbContext> options) : base(options) { }

        public DbSet<Contratacao> Contratacoes { get; set; } = null!;
        public DbSet<PropostaStatusReadModel> PropostaStatuses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contratacao>(b =>
            {
                b.ToTable("contratacoes");
                b.HasKey(x => x.Id);
                b.Property(x => x.PropostaId).IsRequired().HasColumnName("proposta_id");
                b.Property(x => x.DataContratacao).HasColumnName("data_contratacao");
            });

            modelBuilder.Entity<PropostaStatusReadModel>(b =>
            {
                b.ToTable("proposta_statuses");
                b.HasKey(x => x.PropostaId);
                b.Property(x => x.Status).IsRequired().HasColumnName("status");
                b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            });
        }
    }

    public class PropostaStatusReadModel
    {
        public Guid PropostaId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
    }
}
