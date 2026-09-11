using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using InsureFlow.PropostaService.Application.UseCases;
using InsureFlow.PropostaService.Application.Ports;
using InsureFlow.PropostaService.Domain.Entities;

namespace PropostaService.UnitTests
{
    public class CriarPropostaUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_ValidData_CallsRepositoryAndReturnsSuccess()
        {
            var mockRepo = new Mock<IPropostaRepository>();
            mockRepo
                .Setup(r => r.AddAsync(It.IsAny<Proposta>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask)
                .Verifiable();

            var useCase = new CriarPropostaUseCase(mockRepo.Object);

            var result = await useCase.ExecuteAsync("Mariana", "Vida", 30000m, 150m);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.NomeSegurado.Should().Be("Mariana");

            mockRepo.Verify(r => r.AddAsync(It.IsAny<Proposta>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_InvalidData_ReturnsFailure()
        {
            var mockRepo = new Mock<IPropostaRepository>();
            var useCase = new CriarPropostaUseCase(mockRepo.Object);

            var result = await useCase.ExecuteAsync("", "Vida", 0m, 0m);

            result.IsFailure.Should().BeTrue();
            mockRepo.Verify(r => r.AddAsync(It.IsAny<Proposta>()), Times.Never);
        }
    }
}
