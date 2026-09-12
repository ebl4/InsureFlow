using Microsoft.AspNetCore.Mvc;
using InsureFlow.ContratacaoService.Application.UseCases;
using InsureFlow.ContratacaoService.Application.Ports;
using InsureFlow.ContratacaoService.Domain.Entities;

namespace InsureFlow.ContratacaoService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContratacoesController : ControllerBase
    {
        private readonly ContratarPropostaUseCase _contratarUseCase;
        private readonly IContratacaoRepository _repository;

        public ContratacoesController(ContratarPropostaUseCase contratarUseCase, IContratacaoRepository repository)
        {
            _contratarUseCase = contratarUseCase;
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateContratacaoRequest req)
        {
            try
            {
                var c = await _contratarUseCase.ExecuteAsync(req.PropostaId);
                return CreatedAtAction(nameof(GetById), new { id = c.Id }, new ContratacaoResponse(c));
            }
            catch (Application.Exceptions.PropostaNaoAprovadaException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                // Upstream service unreachable
                return StatusCode(503, new { message = "Proposta service unavailable.", detail = ex.Message });
            }
            catch (Polly.CircuitBreaker.BrokenCircuitException ex)
            {
                // Circuit breaker open
                return StatusCode(503, new { message = "Proposta service circuit open.", detail = ex.Message });
            }
            catch (Exception ex)
            {
                // fallback - internal server error
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return NotFound();
            return Ok(new ContratacaoResponse(c));
        }

        [HttpGet("proposta/{propostaId:guid}")]
        public async Task<IActionResult> GetByProposta(Guid propostaId)
        {
            var c = await _repository.GetByPropostaIdAsync(propostaId);
            if (c == null) return NotFound();
            return Ok(new ContratacaoResponse(c));
        }
    }

    public record CreateContratacaoRequest(Guid PropostaId);

    public record ContratacaoResponse(Guid Id, Guid PropostaId, DateTime DataContratacao)
    {
        public ContratacaoResponse(Contratacao c) : this(c.Id, c.PropostaId, c.DataContratacao) { }
    }
}
