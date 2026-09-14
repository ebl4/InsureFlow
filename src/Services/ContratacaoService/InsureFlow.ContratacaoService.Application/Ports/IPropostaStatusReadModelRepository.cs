namespace InsureFlow.ContratacaoService.Application.Ports
{
    public interface IPropostaStatusReadModelRepository
    {
        Task<string?> GetStatusAsync(Guid propostaId);
        Task UpsertStatusAsync(Guid propostaId, string status);
    }
}
