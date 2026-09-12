using System;
using System.Threading.Tasks;
using InsureFlow.Shared.Kernel;
using InsureFlow.PropostaService.Application.Ports;
using InsureFlow.PropostaService.Domain.Entities;

namespace InsureFlow.PropostaService.Application.UseCases
{
    public class AlterarStatusPropostaUseCase
    {
        private readonly IPropostaRepository _repository;

        public AlterarStatusPropostaUseCase(IPropostaRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Proposta>> ExecuteAsync(Guid propostaId, string novoStatus)
        {
            try
            {
                var p = await _repository.GetByIdAsync(propostaId);
                if (p == null) return Result<Proposta>.Fail("not_found");

                if (string.Equals(novoStatus, "Aprovada", StringComparison.OrdinalIgnoreCase))
                {
                    p.Aprovar();
                }
                else if (string.Equals(novoStatus, "Rejeitada", StringComparison.OrdinalIgnoreCase))
                {
                    p.Rejeitar();
                }
                else
                {
                    return Result<Proposta>.Fail("invalid_status");
                }

                await _repository.UpdateAsync(p);
                return Result<Proposta>.Ok(p);
            }
            catch (InvalidOperationException ex)
            {
                return Result<Proposta>.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<Proposta>.Fail(ex.Message);
            }
        }
    }
}
