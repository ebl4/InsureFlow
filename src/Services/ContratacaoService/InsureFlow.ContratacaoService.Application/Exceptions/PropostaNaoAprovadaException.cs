namespace InsureFlow.ContratacaoService.Application.Exceptions
{
    public class PropostaNaoAprovadaException : Exception
    {
        public PropostaNaoAprovadaException(Guid propostaId)
            : base($"Proposal {propostaId} is not approved and cannot be contracted.")
        {
        }
    }
}
