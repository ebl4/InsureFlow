using InsureFlow.ContratacaoService.Domain.Entities;

namespace InsureFlow.ContratacaoService.Application.Services
{
    public interface IContratacaoService
    {
        Task<Contratacao> ContratarAsync(Guid propostaId);
        Task<Contratacao?> GetByIdAsync(Guid id);
        Task<Contratacao?> GetByPropostaIdAsync(Guid propostaId);
    }
}
