using InsureFlow.PropostaService.Application.Ports;
using InsureFlow.PropostaService.Application.UseCases;
using InsureFlow.PropostaService.Domain.Entities;
using InsureFlow.Shared.Kernel;

namespace InsureFlow.PropostaService.Application.Services
{
    public class PropostaAppService : IPropostaService
    {
        private readonly CriarPropostaUseCase _criar;
        private readonly AlterarStatusPropostaUseCase _alterar;
        private readonly IPropostaRepository _repo;

        public PropostaAppService(CriarPropostaUseCase criar, AlterarStatusPropostaUseCase alterar, IPropostaRepository repo)
        {
            _criar = criar;
            _alterar = alterar;
            _repo = repo;
        }

        public Task<Result<Proposta>> CreateAsync(string nomeSegurado, string tipoSeguro, decimal valorCobertura, decimal premioMensal)
            => _criar.ExecuteAsync(nomeSegurado, tipoSeguro, valorCobertura, premioMensal);

        public Task<Result<Proposta>> AlterarStatusAsync(Guid propostaId, string novoStatus)
            => _alterar.ExecuteAsync(propostaId, novoStatus);

        public Task<Proposta?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        public Task<IEnumerable<Proposta>> ListAsync() => _repo.ListAsync();
    }
}
