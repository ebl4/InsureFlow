using InsureFlow.PropostaService.Domain.Entities;

namespace InsureFlow.PropostaService.Application.Ports
{
    public interface IPropostaRepository
    {
        Task AddAsync(Proposta proposta);
        Task<Proposta?> GetByIdAsync(Guid id);
        Task UpdateAsync(Proposta proposta);
        Task<IEnumerable<Proposta>> ListAsync();
    }
}
