using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using InsureFlow.ContratacaoService.Api.Controllers;
using InsureFlow.ContratacaoService.Application.Services;
using InsureFlow.ContratacaoService.Domain.Entities;
using InsureFlow.ContratacaoService.Application.Exceptions;

namespace InsureFlow.ContratacaoService.UnitTests.Controllers
{
    public class ContratacoesControllerTests
    {
        [Fact]
        public async Task Post_ReturnsCreated_WhenContratacaoSucceeds()
        {
            var propostaId = Guid.NewGuid();
            var contratacao = Contratacao.Criar(propostaId);

            var serviceMock = new Mock<IContratacaoService>();
            serviceMock.Setup(s => s.ContratarAsync(propostaId)).ReturnsAsync(contratacao);

            var controller = new ContratacoesController(serviceMock.Object);
            var res = await controller.Post(new CreateContratacaoRequest(propostaId));

            res.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task Post_ReturnsConflict_WhenPropostaNotApproved()
        {
            var propostaId = Guid.NewGuid();
            var serviceMock = new Mock<IContratacaoService>();
            serviceMock.Setup(s => s.ContratarAsync(propostaId)).ThrowsAsync(new PropostaNaoAprovadaException(propostaId));

            var controller = new ContratacoesController(serviceMock.Object);
            var res = await controller.Post(new CreateContratacaoRequest(propostaId));

            res.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNull()
        {
            var id = Guid.NewGuid();
            var serviceMock = new Mock<IContratacaoService>();
            serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((Contratacao?)null);

            var controller = new ContratacoesController(serviceMock.Object);
            var res = await controller.GetById(id);

            res.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetByProposta_ReturnsOk_WhenFound()
        {
            var propostaId = Guid.NewGuid();
            var contratacao = Contratacao.Criar(propostaId);
            var serviceMock = new Mock<IContratacaoService>();
            serviceMock.Setup(s => s.GetByPropostaIdAsync(propostaId)).ReturnsAsync(contratacao);

            var controller = new ContratacoesController(serviceMock.Object);
            var res = await controller.GetByProposta(propostaId);

            res.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Post_PropagatesException_WhenUnexpectedError()
        {
            var propostaId = Guid.NewGuid();
            var serviceMock = new Mock<IContratacaoService>();
            serviceMock.Setup(s => s.ContratarAsync(propostaId)).ThrowsAsync(new InvalidOperationException("boom"));

            var controller = new ContratacoesController(serviceMock.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => controller.Post(new CreateContratacaoRequest(propostaId)));
        }
    }
}
