using Polly;
using Polly.Extensions.Http;
using InsureFlow.ContratacaoService.Application.UseCases;
using InsureFlow.ContratacaoService.Application.Ports;
using InsureFlow.ContratacaoService.Infrastructure.Clients;
using InsureFlow.ContratacaoService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using InsureFlow.ContratacaoService.Application.Services;
using InsureFlow.ContratacaoService.Infrastructure.RabbitMq;

internal class Program
{
    private static void Main(string[] args)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // DI
        var connStr = builder.Configuration.GetConnectionString("ContratacaoDatabase") ?? builder.Configuration["ConnectionStrings:ContratacaoDatabase"];
        if (!string.IsNullOrEmpty(connStr))
        {
            builder.Services.AddDbContext<ContratacaoDbContext>(options =>
                options.UseNpgsql(connStr).UseSnakeCaseNamingConvention());
            builder.Services.AddScoped<IContratacaoRepository, ContratacaoEfRepository>();
            builder.Services.AddScoped<IPropostaStatusReadModelRepository, PropostaStatusRepository>();
            builder.Services.AddScoped<IPropostaStatusRepository>(sp => (PropostaStatusRepository)sp.GetRequiredService<IPropostaStatusReadModelRepository>());
        }
        else
        {
            builder.Services.AddSingleton<IContratacaoRepository, InMemoryContratacaoRepository>();
        }

        builder.Services.AddTransient<ContratarPropostaUseCase>();
        builder.Services.AddTransient<IContratacaoService, ContratacaoAppService>();

        // RabbitMQ consumer
        var rabbitHost = builder.Configuration["RabbitMQ:HostName"] ?? builder.Configuration["RabbitMQ__HostName"] ?? "localhost";
        builder.Services.AddSingleton<IHostedService>(sp =>
            new PropostaStatusConsumer(rabbitHost, sp.GetRequiredService<IServiceScopeFactory>()));

        // PropostaService HttpClient with Polly
        var propostaBase = builder.Configuration["PropostaService:BaseUrl"] ?? "http://localhost:5000";
        builder.Services.AddHttpClient<IPropostaServiceClient, PropostaServiceClient>(client =>
        {
            client.BaseAddress = new Uri(propostaBase);
        })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

        var app = builder.Build();

        // Ensure database created when using EF
        if (!string.IsNullOrEmpty(connStr))
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ContratacaoDbContext>();
            db.Database.EnsureCreated();
        }

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