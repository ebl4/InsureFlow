using System;
using System.Threading.Tasks;
using InsureFlow.ContratacaoService.Application.Exceptions;
using InsureFlow.ContratacaoService.Application.Ports;
using InsureFlow.ContratacaoService.Domain.Entities;

namespace InsureFlow.ContratacaoService.Application.UseCases
{
    public class ContratarPropostaUseCase
    {
        private readonly IPropostaServiceClient _propostaClient;
        private readonly IContratacaoRepository _repository;

        public ContratarPropostaUseCase(IPropostaServiceClient propostaClient, IContratacaoRepository repository)
        {
            _propostaClient = propostaClient;
            _repository = repository;
        }

        public async Task<Contratacao> ExecuteAsync(Guid propostaId)
        {
            var status = await _propostaClient.GetPropostaStatusAsync(propostaId);
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
