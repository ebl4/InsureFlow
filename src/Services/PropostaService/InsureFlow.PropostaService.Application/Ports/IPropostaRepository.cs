using InsureFlow.PropostaService.Domain.Entities;

namespace InsureFlow.PropostaService.Application.Ports
{
    public interface IPropostaRepository
    {
        Task AddAsync(Proposta proposta);
        Task<Proposta?> GetByIdAsync(Guid id);
        Task<IEnumerable<Proposta>> ListAsync();
    }
}
