using System;
using System.Threading.Tasks;
using InsureFlow.ContratacaoService.Domain.Entities;

namespace InsureFlow.ContratacaoService.Application.Ports
{
    public interface IContratacaoRepository
    {
        Task AddAsync(Contratacao contratacao);
        Task<Contratacao?> GetByIdAsync(Guid id);
        Task<Contratacao?> GetByPropostaIdAsync(Guid propostaId);
    }
}
