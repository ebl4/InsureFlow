using Microsoft.AspNetCore.Mvc;
using InsureFlow.PropostaService.Application.UseCases;
using InsureFlow.PropostaService.Application.Ports;
using InsureFlow.PropostaService.Domain.Entities;

namespace InsureFlow.PropostaService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropostasController : ControllerBase
    {
        private readonly CriarPropostaUseCase _criarUseCase;
        private readonly IPropostaRepository _repository;

        public PropostasController(CriarPropostaUseCase criarUseCase, IPropostaRepository repository)
        {
            _criarUseCase = criarUseCase;
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreatePropostaRequest request)
        {
            var result = await _criarUseCase.ExecuteAsync(request.NomeSegurado, request.TipoSeguro, request.ValorCobertura, request.PremioMensal);
            if (result.IsFailure) return BadRequest(result.Error);
            var p = result.Value!;
            return CreatedAtAction(nameof(GetById), new { id = p.Id }, new PropostaResponse(p));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _repository.ListAsync();
            var resp = list.Select(p => new PropostaResponse(p));
            return Ok(resp);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var p = await _repository.GetByIdAsync(id);
            if (p == null) return NotFound();
            return Ok(new PropostaResponse(p));
        }
    }

    public record CreatePropostaRequest(string NomeSegurado, string TipoSeguro, decimal ValorCobertura, decimal PremioMensal);

    public record PropostaResponse(Guid Id, string NomeSegurado, string TipoSeguro, decimal ValorCobertura, decimal PremioMensal, string Status, DateTime DataCriacao)
    {
        public PropostaResponse(Proposta p)
            : this(p.Id, p.NomeSegurado, p.TipoSeguro, p.ValorCobertura, p.PremioMensal, p.Status.ToString(), p.DataCriacao)
        {
        }
    }
}
