using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using InsureFlow.ContratacaoService.Infrastructure.Clients;
using Xunit;

namespace InsureFlow.ContratacaoService.UnitTests.Clients
{
    public class PropostaServiceClientTests
    {
        [Fact]
        public async Task GetPropostaStatusAsync_ReturnsStatus_WhenResponseIsOk()
        {
            var propostaId = Guid.NewGuid();
            var json = $"{ {"Id":"{propostaId}","Status":"Aprovada"} }";

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{ \"id\": \"" + propostaId + "\", \"status\": \"Aprovada\" }")
                });

            var client = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost") };
            var sut = new PropostaServiceClient(client);

            var status = await sut.GetPropostaStatusAsync(propostaId);

            status.Should().Be("Aprovada");
        }

        [Fact]
        public async Task GetPropostaStatusAsync_ReturnsNull_WhenNotFound()
        {
            var propostaId = Guid.NewGuid();

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var client = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost") };
            var sut = new PropostaServiceClient(client);

            var status = await sut.GetPropostaStatusAsync(propostaId);

            status.Should().BeNull();
        }
    }
}
