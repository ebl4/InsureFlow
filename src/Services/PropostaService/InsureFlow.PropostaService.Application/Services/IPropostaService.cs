using InsureFlow.PropostaService.Domain.Entities;
using InsureFlow.Shared.Kernel;

namespace InsureFlow.PropostaService.Application.Services
{
    public interface IPropostaService
    {
        Task<Result<Proposta>> CreateAsync(string nomeSegurado, string tipoSeguro, decimal valorCobertura, decimal premioMensal);
        Task<Result<Proposta>> AlterarStatusAsync(Guid propostaId, string novoStatus);
        Task<Proposta?> GetByIdAsync(Guid id);
        Task<IEnumerable<Proposta>> ListAsync();
    }
}
