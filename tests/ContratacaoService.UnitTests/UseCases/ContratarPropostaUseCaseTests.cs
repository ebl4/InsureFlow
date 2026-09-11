using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using InsureFlow.ContratacaoService.Application.Ports;
using InsureFlow.ContratacaoService.Application.UseCases;
using InsureFlow.ContratacaoService.Application.Exceptions;
using InsureFlow.ContratacaoService.Domain.Entities;
using Xunit;

namespace InsureFlow.ContratacaoService.UnitTests.UseCases
{
    public class ContratarPropostaUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_WhenPropostaApproved_PersistsContratacao()
        {
            var propostaId = Guid.NewGuid();
            var propostaClient = new Mock<IPropostaServiceClient>();
            propostaClient.Setup(x => x.GetPropostaStatusAsync(propostaId)).ReturnsAsync("Aprovada");

            Contratacao? saved = null;
            var repo = new Mock<IContratacaoRepository>();
            repo.Setup(r => r.AddAsync(It.IsAny<Contratacao>())).Callback<Contratacao>(c => saved = c).Returns(Task.CompletedTask);

            var useCase = new ContratarPropostaUseCase(propostaClient.Object, repo.Object);

            var result = await useCase.ExecuteAsync(propostaId);

            result.Should().NotBeNull();
            result.PropostaId.Should().Be(propostaId);
            saved.Should().NotBeNull();
            saved!.Id.Should().Be(result.Id);
        }

        [Fact]
        public async Task ExecuteAsync_WhenPropostaNotApproved_ThrowsException()
        {
            var propostaId = Guid.NewGuid();
            var propostaClient = new Mock<IPropostaServiceClient>();
            propostaClient.Setup(x => x.GetPropostaStatusAsync(propostaId)).ReturnsAsync((string?)"Rejeitada");

            var repo = new Mock<IContratacaoRepository>();

            var useCase = new ContratarPropostaUseCase(propostaClient.Object, repo.Object);

            await Assert.ThrowsAsync<PropostaNaoAprovadaException>(() => useCase.ExecuteAsync(propostaId));
            repo.Verify(r => r.AddAsync(It.IsAny<Contratacao>()), Times.Never);
        }
    }
}
