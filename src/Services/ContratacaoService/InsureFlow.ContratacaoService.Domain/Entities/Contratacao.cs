namespace InsureFlow.ContratacaoService.Domain.Entities
{
    public class Contratacao
    {
        public Guid Id { get; private set; }
        public Guid PropostaId { get; private set; }
        public DateTime DataContratacao { get; private set; }

        private Contratacao() { }

        private Contratacao(Guid id, Guid propostaId, DateTime dataContratacao)
        {
            Id = id;
            PropostaId = propostaId;
            DataContratacao = dataContratacao;
        }

        public static Contratacao Criar(Guid propostaId)
        {
            return new Contratacao(Guid.NewGuid(), propostaId, DateTime.UtcNow);
        }
    }
}
