using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace InsureFlow.PropostaService.UnitTests.Integration
{
    public class PropostaApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public PropostaApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Create_Then_ChangeStatus_Then_Get_ReturnsUpdatedStatus()
        {
            var client = _factory.CreateClient();

            var createReq = new { NomeSegurado = "Jane Doe", TipoSeguro = "Home", ValorCobertura = 50000m, PremioMensal = 50m };
            var createRes = await client.PostAsJsonAsync("/api/propostas", createReq);
            createRes.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await createRes.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            string id = created.GetProperty("id").GetString()!;

            // Patch status to Aprovada
            var patchRes = await client.PatchAsJsonAsync($"/api/propostas/{id}/status", new { status = "Aprovada" });
            patchRes.StatusCode.Should().Be(HttpStatusCode.OK);

            var getRes = await client.GetAsync($"/api/propostas/{id}");
            getRes.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await getRes.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            string status = body.GetProperty("status").GetString()!;
            status.Should().Be("Aprovada");
        }
    }
}
