using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using InsureFlow.ContratacaoService.Application.Ports;

namespace InsureFlow.ContratacaoService.UnitTests.Integration
{
    public class ContratacaoApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ContratacaoApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace IPropostaServiceClient with a fake that always returns Aprovada
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPropostaServiceClient));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddSingleton<IPropostaServiceClient>(new FakeApprovedClient());
                });
            });
        }

        [Fact]
        public async Task Post_CreateContratacao_ReturnsCreated_AndCanGetById()
        {
            var client = _factory.CreateClient();

            var propostaId = Guid.NewGuid();
            var res = await client.PostAsJsonAsync("/api/contratacoes", new { propostaId });
            res.StatusCode.Should().Be(HttpStatusCode.Created);

            var location = res.Headers.Location;
            location.Should().NotBeNull();

            var get = await client.GetAsync(location!);
            get.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await get.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            body.GetProperty("propostaId").GetGuid().Should().Be(propostaId);
        }

        private class FakeApprovedClient : IPropostaServiceClient
        {
            public Task<string?> GetPropostaStatusAsync(Guid propostaId) => Task.FromResult<string?>("Aprovada");
        }
    }
}
