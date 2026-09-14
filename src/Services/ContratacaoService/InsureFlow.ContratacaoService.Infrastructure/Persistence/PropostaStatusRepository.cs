namespace InsureFlow.ContratacaoService.Infrastructure.Persistence
{
    public class PropostaStatusRepository : IPropostaStatusRepository, Application.Ports.IPropostaStatusReadModelRepository
    {
        private readonly ContratacaoDbContext _db;

        public PropostaStatusRepository(ContratacaoDbContext db)
        {
            _db = db;
        }

        public async Task<string?> GetStatusAsync(Guid propostaId)
        {
            var r = await _db.PropostaStatuses.FindAsync(propostaId);
            return r?.Status;
        }

        public async Task UpsertStatusAsync(Guid propostaId, string status)
        {
            var r = await _db.PropostaStatuses.FindAsync(propostaId);
            if (r == null)
            {
                r = new PropostaStatusReadModel { PropostaId = propostaId, Status = status, UpdatedAt = DateTime.UtcNow };
                _db.PropostaStatuses.Add(r);
            }
            else
            {
                r.Status = status;
                r.UpdatedAt = DateTime.UtcNow;
                _db.PropostaStatuses.Update(r);
            }

            await _db.SaveChangesAsync();
        }
    }

}
