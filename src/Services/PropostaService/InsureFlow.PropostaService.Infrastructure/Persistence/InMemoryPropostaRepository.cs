using System.Collections.Concurrent;
using InsureFlow.PropostaService.Application.Ports;
using InsureFlow.PropostaService.Domain.Entities;

namespace InsureFlow.PropostaService.Infrastructure.Persistence
{
    public class InMemoryPropostaRepository : IPropostaRepository
    {
        private readonly ConcurrentDictionary<Guid, Proposta> _store = new();

        public Task AddAsync(Proposta proposta)
        {
            _store[proposta.Id] = proposta;
            return Task.CompletedTask;
        }

        public Task<Proposta?> GetByIdAsync(Guid id)
        {
            _store.TryGetValue(id, out var proposta);
            return Task.FromResult(proposta);
        }

        public Task UpdateAsync(Proposta proposta)
        {
            _store[proposta.Id] = proposta;
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Proposta>> ListAsync()
        {
            var list = _store.Values.ToList();
            return Task.FromResult<IEnumerable<Proposta>>(list);
        }
    }
}
