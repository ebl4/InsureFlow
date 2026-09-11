using System.Collections.Concurrent;
using InsureFlow.ContratacaoService.Application.Ports;
using InsureFlow.ContratacaoService.Domain.Entities;

namespace InsureFlow.ContratacaoService.Infrastructure.Persistence
{
    public class InMemoryContratacaoRepository : IContratacaoRepository
    {
        private readonly ConcurrentDictionary<Guid, Contratacao> _store = new();

        public Task AddAsync(Contratacao contratacao)
        {
            _store[contratacao.Id] = contratacao;
            return Task.CompletedTask;
        }

        public Task<Contratacao?> GetByIdAsync(Guid id)
        {
            _store.TryGetValue(id, out var c);
            return Task.FromResult(c);
        }

        public Task<Contratacao?> GetByPropostaIdAsync(Guid propostaId)
        {
            var c = _store.Values.FirstOrDefault(x => x.PropostaId == propostaId);
            return Task.FromResult(c);
        }
    }
}
