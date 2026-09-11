using InsureFlow.PropostaService.Application.UseCases;
using InsureFlow.PropostaService.Application.Ports;
using InsureFlow.PropostaService.Infrastructure.Persistence;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // DI registrations
        builder.Services.AddSingleton<IPropostaRepository, InMemoryPropostaRepository>();
        builder.Services.AddTransient<CriarPropostaUseCase>();

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
    }
}