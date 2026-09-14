using InsureFlow.PropostaService.Application.Ports;
using InsureFlow.PropostaService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InsureFlow.PropostaService.Infrastructure.Persistence
{
    public class PropostaEfRepository : IPropostaRepository
    {
        private readonly PropostaDbContext _db;

        public PropostaEfRepository(PropostaDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Proposta proposta)
        {
            _db.Propostas.Add(proposta);
            await _db.SaveChangesAsync();
        }

        public async Task<Proposta?> GetByIdAsync(Guid id)
        {
            return await _db.Propostas.FindAsync(id);
        }

        public async Task UpdateAsync(Proposta proposta)
        {
            _db.Propostas.Update(proposta);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Proposta>> ListAsync()
        {
            return await _db.Propostas.ToListAsync();
        }
    }
}
