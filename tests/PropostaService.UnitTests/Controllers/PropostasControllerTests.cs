using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using InsureFlow.PropostaService.Api.Controllers;
using InsureFlow.PropostaService.Application.Services;
using InsureFlow.PropostaService.Domain.Entities;

namespace InsureFlow.PropostaService.UnitTests.Controllers
{
    public class PropostasControllerTests
    {
        [Fact]
        public async Task Post_ReturnsCreated_WhenServiceCreates()
        {
            var proposta = Proposta.Criar("John", "Auto", 1000, 100);
            var serviceMock = new Mock<IPropostaService>();
            serviceMock.Setup(s => s.CreateAsync(proposta.NomeSegurado, proposta.TipoSeguro, proposta.ValorCobertura, proposta.PremioMensal))
                .ReturnsAsync(Shared.Kernel.Result<Proposta>.Ok(proposta));

            var controller = new PropostasController(serviceMock.Object);

            var req = new CreatePropostaRequest(proposta.NomeSegurado, proposta.TipoSeguro, proposta.ValorCobertura, proposta.PremioMensal);
            var res = await controller.Post(req);

            res.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task PatchStatus_ReturnsNotFound_WhenServiceReturnsNotFound()
        {
            var id = Guid.NewGuid();
            var serviceMock = new Mock<IPropostaService>();
            serviceMock.Setup(s => s.AlterarStatusAsync(id, "Aprovada")).ReturnsAsync(InsureFlow.Shared.Kernel.Result<Proposta>.Fail("not_found"));

            var controller = new PropostasController(serviceMock.Object);
            var res = await controller.PatchStatus(id, new AlterarStatusRequest("Aprovada"));

            res.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task PatchStatus_ReturnsBadRequest_WhenInvalidStatus()
        {
            var id = Guid.NewGuid();
            var serviceMock = new Mock<IPropostaService>();
            serviceMock.Setup(s => s.AlterarStatusAsync(id, "Xyz")).ReturnsAsync(InsureFlow.Shared.Kernel.Result<Proposta>.Fail("invalid_status"));

            var controller = new PropostasController(serviceMock.Object);
            var res = await controller.PatchStatus(id, new AlterarStatusRequest("Xyz"));

            res.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PatchStatus_ReturnsOk_WhenSuccess()
        {
            var proposta = Proposta.Criar("John", "Auto", 1000, 100);
            var serviceMock = new Mock<IPropostaService>();
            serviceMock.Setup(s => s.AlterarStatusAsync(proposta.Id, "Aprovada")).ReturnsAsync(InsureFlow.Shared.Kernel.Result<Proposta>.Ok(proposta));

            var controller = new PropostasController(serviceMock.Object);
            var res = await controller.PatchStatus(proposta.Id, new AlterarStatusRequest("Aprovada"));

            res.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Get_ReturnsOk_WithEmptyList()
        {
            var serviceMock = new Mock<IPropostaService>();
            serviceMock.Setup(s => s.ListAsync()).ReturnsAsync(new Proposta[0]);

            var controller = new PropostasController(serviceMock.Object);
            var res = await controller.Get();

            res.Should().BeOfType<OkObjectResult>();
        }
    }
}
