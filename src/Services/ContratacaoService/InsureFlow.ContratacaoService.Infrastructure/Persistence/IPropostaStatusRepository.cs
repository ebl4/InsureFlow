namespace InsureFlow.ContratacaoService.Infrastructure.Persistence
{
    public interface IPropostaStatusRepository
    {
        Task<string?> GetStatusAsync(Guid propostaId);
        Task UpsertStatusAsync(Guid propostaId, string status);
    }
}
