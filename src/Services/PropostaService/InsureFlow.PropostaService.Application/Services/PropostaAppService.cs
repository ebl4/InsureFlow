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
        private readonly IEventPublisher? _publisher;

        public PropostaAppService(CriarPropostaUseCase criar, AlterarStatusPropostaUseCase alterar, IPropostaRepository repo, IEventPublisher? publisher = null)
        {
            _criar = criar;
            _alterar = alterar;
            _repo = repo;
            _publisher = publisher;
        }

        public Task<Result<Proposta>> CreateAsync(string nomeSegurado, string tipoSeguro, decimal valorCobertura, decimal premioMensal)
            => _criar.ExecuteAsync(nomeSegurado, tipoSeguro, valorCobertura, premioMensal);

        public Task<Result<Proposta>> AlterarStatusAsync(Guid propostaId, string novoStatus)
            => ExecuteAndPublishAsync(propostaId, novoStatus);

        private async Task<Result<Proposta>> ExecuteAndPublishAsync(Guid propostaId, string novoStatus)
        {
            var result = await _alterar.ExecuteAsync(propostaId, novoStatus);
            if (result.IsSuccess && _publisher != null)
            {
                var p = result.Value!;
                var @event = new { p.Id, Status = p.Status.ToString(), p.DataAtualizacao };
                try
                {
                    _publisher.Publish("proposta.status", string.Empty, @event);
                }
                catch
                {
                    // swallow publisher errors to not break main flow
                }
            }
            return result;
        }

        public Task<Proposta?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        public Task<IEnumerable<Proposta>> ListAsync() => _repo.ListAsync();
    }
}
