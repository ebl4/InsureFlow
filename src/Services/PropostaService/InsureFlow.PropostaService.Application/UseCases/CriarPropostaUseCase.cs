using InsureFlow.Shared.Kernel;
using InsureFlow.PropostaService.Application.Ports;
using InsureFlow.PropostaService.Domain.Entities;

namespace InsureFlow.PropostaService.Application.UseCases
{
    public class CriarPropostaUseCase
    {
        private readonly IPropostaRepository _repository;

        public CriarPropostaUseCase(IPropostaRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Proposta>> ExecuteAsync(string nomeSegurado, string tipoSeguro, decimal valorCobertura, decimal premioMensal)
        {
            try
            {
                var proposta = Proposta.Criar(nomeSegurado, tipoSeguro, valorCobertura, premioMensal);
                await _repository.AddAsync(proposta);
                return Result<Proposta>.Ok(proposta);
            }
            catch (System.Exception ex)
            {
                return Result<Proposta>.Fail(ex.Message);
            }
        }
    }
}
