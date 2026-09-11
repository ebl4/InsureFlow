using System;
using InsureFlow.Shared.Kernel;

namespace InsureFlow.PropostaService.Domain.Entities
{
    public enum StatusProposta
    {
        EmAnalise = 1,
        Aprovada = 2,
        Rejeitada = 3
    }

    public class Proposta
    {
        public Guid Id { get; private set; }
        public string NomeSegurado { get; private set; }
        public string TipoSeguro { get; private set; }
        public decimal ValorCobertura { get; private set; }
        public decimal PremioMensal { get; private set; }
        public StatusProposta Status { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public DateTime? DataAtualizacao { get; private set; }

        private Proposta(Guid id, string nomeSegurado, string tipoSeguro, decimal valorCobertura, decimal premioMensal)
        {
            Id = id;
            NomeSegurado = nomeSegurado;
            TipoSeguro = tipoSeguro;
            ValorCobertura = valorCobertura;
            PremioMensal = premioMensal;
            Status = StatusProposta.EmAnalise;
            DataCriacao = DateTime.UtcNow;
        }

        public static Proposta Criar(string nomeSegurado, string tipoSeguro, decimal valorCobertura, decimal premioMensal)
        {
            Guard.AgainstNullOrEmpty(nomeSegurado, nameof(nomeSegurado));
            Guard.AgainstNullOrEmpty(tipoSeguro, nameof(tipoSeguro));
            Guard.AgainstNegativeOrZero(valorCobertura, nameof(valorCobertura));
            Guard.AgainstNegativeOrZero(premioMensal, nameof(premioMensal));

            return new Proposta(Guid.NewGuid(), nomeSegurado.Trim(), tipoSeguro.Trim(), valorCobertura, premioMensal);
        }

        public void Aprovar()
        {
            if (Status != StatusProposta.EmAnalise)
                throw new InvalidOperationException("Only proposals in analysis can be approved.");
            Status = StatusProposta.Aprovada;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void Rejeitar()
        {
            if (Status != StatusProposta.EmAnalise)
                throw new InvalidOperationException("Only proposals in analysis can be rejected.");
            Status = StatusProposta.Rejeitada;
            DataAtualizacao = DateTime.UtcNow;
        }
    }
}
