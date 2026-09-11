using System;
using FluentAssertions;
using Xunit;
using InsureFlow.PropostaService.Domain.Entities;

namespace PropostaService.UnitTests
{
    public class PropostaTests
    {
        [Fact]
        public void Criar_ValidData_SetsPropertiesAndStatusEmAnalise()
        {
            var p = Proposta.Criar("Joao Silva", "Auto", 100000m, 250m);

            p.NomeSegurado.Should().Be("Joao Silva");
            p.TipoSeguro.Should().Be("Auto");
            p.ValorCobertura.Should().Be(100000m);
            p.PremioMensal.Should().Be(250m);
            p.Status.Should().Be(StatusProposta.EmAnalise);
            p.DataCriacao.Should().BeOnOrBefore(DateTime.UtcNow);
        }

        [Fact]
        public void Criar_EmptyNome_ThrowsArgumentException()
        {
            Action act = () => Proposta.Criar("", "Vida", 50000m, 120m);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Criar_NegativeValor_ThrowsArgumentException()
        {
            Action act = () => Proposta.Criar("Ana", "Residencial", -1m, 100m);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Aprovar_FromEmAnalise_ChangesStatusAndPreventsSecondTransition()
        {
            var p = Proposta.Criar("Carlos", "Auto", 20000m, 80m);
            p.Aprovar();
            p.Status.Should().Be(StatusProposta.Aprovada);

            Action second = () => p.Aprovar();
            second.Should().Throw<InvalidOperationException>();
        }
    }
}
