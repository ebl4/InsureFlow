using System;
using System.Threading.Tasks;

namespace InsureFlow.ContratacaoService.Application.Ports
{
    public interface IPropostaServiceClient
    {
        Task<string?> GetPropostaStatusAsync(Guid propostaId);
    }
}
