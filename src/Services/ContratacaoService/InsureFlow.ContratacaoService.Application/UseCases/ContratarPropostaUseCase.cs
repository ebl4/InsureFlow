using InsureFlow.ContratacaoService.Application.Exceptions;
using InsureFlow.ContratacaoService.Application.Ports;
using InsureFlow.ContratacaoService.Domain.Entities;

namespace InsureFlow.ContratacaoService.Application.UseCases
{
    public class ContratarPropostaUseCase
    {
        private readonly IPropostaServiceClient _propostaClient;
        private readonly IContratacaoRepository _repository;
        private readonly IPropostaStatusReadModelRepository? _statusReadModel;

        public ContratarPropostaUseCase(IPropostaServiceClient propostaClient, IContratacaoRepository repository, IPropostaStatusReadModelRepository? statusReadModel = null)
        {
            _propostaClient = propostaClient;
            _repository = repository;
            _statusReadModel = statusReadModel;
        }

        public async Task<Contratacao> ExecuteAsync(Guid propostaId)
        {
            string? status = null;
            if (_statusReadModel != null)
            {
                status = await _statusReadModel.GetStatusAsync(propostaId);
            }

            if (status == null)
            {
                status = await _propostaClient.GetPropostaStatusAsync(propostaId);
            }

            if (status == null || !string.Equals(status, "Aprovada", StringComparison.OrdinalIgnoreCase))
            {
                throw new PropostaNaoAprovadaException(propostaId);
            }

            var contratacao = Contratacao.Criar(propostaId);
            await _repository.AddAsync(contratacao);
            return contratacao;
        }
    }
}
