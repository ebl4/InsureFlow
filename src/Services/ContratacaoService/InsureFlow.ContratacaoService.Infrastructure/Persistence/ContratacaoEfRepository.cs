using InsureFlow.ContratacaoService.Application.Ports;
using InsureFlow.ContratacaoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.ContratacaoService.Infrastructure.Persistence
{
    public class ContratacaoEfRepository : IContratacaoRepository
    {
        private readonly ContratacaoDbContext _db;

        public ContratacaoEfRepository(ContratacaoDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Contratacao contratacao)
        {
            _db.Contratacoes.Add(contratacao);
            await _db.SaveChangesAsync();
        }

        public async Task<Contratacao?> GetByIdAsync(Guid id)
        {
            return await _db.Contratacoes.FindAsync(id);
        }

        public async Task<Contratacao?> GetByPropostaIdAsync(Guid propostaId)
        {
            return await _db.Contratacoes.FirstOrDefaultAsync(x => x.PropostaId == propostaId);
        }
    }
}
