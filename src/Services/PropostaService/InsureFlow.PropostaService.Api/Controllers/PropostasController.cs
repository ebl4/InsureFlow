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
        private readonly AlterarStatusPropostaUseCase _alterarStatusUseCase;
        private readonly IPropostaRepository _repository;

        public PropostasController(CriarPropostaUseCase criarUseCase, IPropostaRepository repository, AlterarStatusPropostaUseCase alterarStatusUseCase)
        {
            _criarUseCase = criarUseCase;
            _alterarStatusUseCase = alterarStatusUseCase;
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

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> PatchStatus(Guid id, AlterarStatusRequest req)
        {
            var result = await _alterarStatusUseCase.ExecuteAsync(id, req.Status);
            if (result.IsFailure)
            {
                if (result.Error == "not_found") return NotFound();
                if (result.Error == "invalid_status") return BadRequest(new { message = "Invalid status. Use 'Aprovada' or 'Rejeitada'." });
                return BadRequest(result.Error);
            }

            return Ok(new PropostaResponse(result.Value!));
        }
    }

    public record CreatePropostaRequest(string NomeSegurado, string TipoSeguro, decimal ValorCobertura, decimal PremioMensal);

    public record AlterarStatusRequest(string Status);

    public record PropostaResponse(Guid Id, string NomeSegurado, string TipoSeguro, decimal ValorCobertura, decimal PremioMensal, string Status, DateTime DataCriacao)
    {
        public PropostaResponse(Proposta p)
            : this(p.Id, p.NomeSegurado, p.TipoSeguro, p.ValorCobertura, p.PremioMensal, p.Status.ToString(), p.DataCriacao)
        {
        }
    }
}
