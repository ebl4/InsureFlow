using Polly;
using Polly.Extensions.Http;
using InsureFlow.ContratacaoService.Application.UseCases;
using InsureFlow.ContratacaoService.Application.Ports;
using InsureFlow.ContratacaoService.Infrastructure.Clients;
using InsureFlow.ContratacaoService.Infrastructure.Persistence;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // DI
        builder.Services.AddSingleton<IContratacaoRepository, InMemoryContratacaoRepository>();
        builder.Services.AddTransient<ContratarPropostaUseCase>();

        // PropostaService HttpClient with Polly
        var propostaBase = builder.Configuration["PropostaService:BaseUrl"] ?? "http://localhost:5000";
        builder.Services.AddHttpClient<IPropostaServiceClient, PropostaServiceClient>(client =>
        {
            client.BaseAddress = new Uri(propostaBase);
        })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();

        IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3) });
        }

        IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30));
        }
    }
}