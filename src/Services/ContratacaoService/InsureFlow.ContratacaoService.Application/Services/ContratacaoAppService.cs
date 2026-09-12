using InsureFlow.ContratacaoService.Application.Ports;
using InsureFlow.ContratacaoService.Application.UseCases;
using InsureFlow.ContratacaoService.Domain.Entities;

namespace InsureFlow.ContratacaoService.Application.Services
{
    public class ContratacaoAppService : IContratacaoService
    {
        private readonly ContratarPropostaUseCase _contratarUseCase;
        private readonly IContratacaoRepository _repo;

        public ContratacaoAppService(ContratarPropostaUseCase contratarUseCase, IContratacaoRepository repo)
        {
            _contratarUseCase = contratarUseCase;
            _repo = repo;
        }

        public Task<Contratacao> ContratarAsync(Guid propostaId)
            => _contratarUseCase.ExecuteAsync(propostaId);

        public Task<Contratacao?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        public Task<Contratacao?> GetByPropostaIdAsync(Guid propostaId) => _repo.GetByPropostaIdAsync(propostaId);
    }
}
