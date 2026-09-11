using System.Text.Json;
using InsureFlow.ContratacaoService.Application.Ports;

namespace InsureFlow.ContratacaoService.Infrastructure.Clients
{
    public class PropostaServiceClient : IPropostaServiceClient
    {
        private readonly HttpClient _http;

        public PropostaServiceClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<string?> GetPropostaStatusAsync(Guid propostaId)
        {
            var resp = await _http.GetAsync($"/api/propostas/{propostaId}");
            if (!resp.IsSuccessStatusCode) return null;
            using var stream = await resp.Content.ReadAsStreamAsync();
            var doc = await JsonSerializer.DeserializeAsync<PropostaDto>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return doc?.Status;
        }

        private class PropostaDto
        {
            public Guid Id { get; set; }
            public string? NomeSegurado { get; set; }
            public string? TipoSeguro { get; set; }
            public decimal ValorCobertura { get; set; }
            public decimal PremioMensal { get; set; }
            public string? Status { get; set; }
            public DateTime DataCriacao { get; set; }
        }
    }
}
