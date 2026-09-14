using InsureFlow.PropostaService.Application.UseCases;
using InsureFlow.PropostaService.Application.Ports;
using InsureFlow.PropostaService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using InsureFlow.PropostaService.Application.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // DI registrations
        var connStr = builder.Configuration.GetConnectionString("PropostaDatabase") ?? builder.Configuration["ConnectionStrings:PropostaDatabase"];
        if (!string.IsNullOrEmpty(connStr))
        {
            builder.Services.AddDbContext<InsureFlow.PropostaService.Infrastructure.Persistence.PropostaDbContext>(options =>
                options.UseNpgsql(connStr));
            builder.Services.AddScoped<InsureFlow.PropostaService.Application.Ports.IPropostaRepository, InsureFlow.PropostaService.Infrastructure.Persistence.PropostaEfRepository>();
        }
        else
        {
            builder.Services.AddSingleton<IPropostaRepository, InMemoryPropostaRepository>();
        }

        builder.Services.AddTransient<CriarPropostaUseCase>();
        builder.Services.AddTransient<AlterarStatusPropostaUseCase>();
        builder.Services.AddTransient<IPropostaService, PropostaAppService>();

        // RabbitMQ publisher
        var rabbitHost = builder.Configuration["RabbitMQ:HostName"] ?? builder.Configuration["RabbitMQ__HostName"] ?? "localhost";
        builder.Services.AddSingleton<IEventPublisher>(new InsureFlow.PropostaService.Infrastructure.RabbitMq.RabbitMqEventPublisher(rabbitHost));

        var app = builder.Build();

        // Ensure database created when using EF
        if (!string.IsNullOrEmpty(connStr))
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InsureFlow.PropostaService.Infrastructure.Persistence.PropostaDbContext>();
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
    }
}