# InsureFlow - Proposta & Contratacao Services

This repository contains microservices for InsureFlow implemented in .NET 8:

- PropostaService: proposal creation and management.
- ContratacaoService: contracts management (created when a proposal is approved).

Prerequisites
- .NET 8 SDK installed (dotnet --version should show 8.x)
- Git
- (Optional) Visual Studio 2022/2026 or another IDE that supports .NET 8

Build
From the repository root (where InsureFlow.sln is located), run:

- Restore and build the solution:
  dotnet build

Run the APIs
You can run each API project directly using the dotnet CLI. From the repository root run:

- PropostaService:

  dotnet run --project src/Services/PropostaService/InsureFlow.PropostaService.Api

- ContratacaoService:

  dotnet run --project src/Services/ContratacaoService/InsureFlow.ContratacaoService.Api

When the applications start they will print the listening URLs to the console. By default you can usually access the APIs at http://localhost:5000 or https://localhost:5001. Swagger UI is enabled in the Development environment at /swagger for both services.

Run tests
To run unit tests for all projects:

  dotnet test

Or target the test projects explicitly:

  dotnet test tests/PropostaService.UnitTests/PropostaService.UnitTests.csproj
  dotnet test tests/ContratacaoService.UnitTests/ContratacaoService.UnitTests.csproj

API Endpoints

PropostaService (base route: /api/propostas)

1) Create a proposal (POST)
- URL: POST /api/propostas
- Request body (JSON):
  {
	"NomeSegurado": "John Doe",
	"TipoSeguro": "Auto",
	"ValorCobertura": 100000.00,
	"PremioMensal": 150.00
  }
- Response: 201 Created with created resource and Location header pointing to GET /api/propostas/{id}

cURL example:

  curl -X POST "http://localhost:5000/api/propostas" \
	-H "Content-Type: application/json" \
	-d '{"NomeSegurado":"John Doe","TipoSeguro":"Auto","ValorCobertura":100000.00,"PremioMensal":150.00}'

2) List proposals (GET)
- URL: GET /api/propostas
- Response: 200 OK with JSON array of proposals

cURL example:

  curl "http://localhost:5000/api/propostas"

3) Get proposal by id (GET)
- URL: GET /api/propostas/{id}
- Response: 200 OK with proposal JSON or 404 Not Found if not found

cURL example:

  curl "http://localhost:5000/api/propostas/{id}"

ContratacaoService (base route: /api/contratacoes)

1) Contract a proposal (POST)
- URL: POST /api/contratacoes
- Request body (JSON):
  { "propostaId": "GUID" }
- Behaviour: the service will call PropostaService to verify the proposal status. Only proposals with Status == "Aprovada" will be contracted. If not approved the service returns 409 Conflict.
- Response: 201 Created with created resource and Location header pointing to GET /api/contratacoes/{id}

cURL example:

  curl -X POST "http://localhost:5001/api/contratacoes" \
	-H "Content-Type: application/json" \
	-d '{"propostaId":"<GUID>"}'

2) Get contract by id (GET)
- URL: GET /api/contratacoes/{id}
- Response: 200 OK with contract JSON or 404 Not Found

3) Get contract by proposal id (GET)
- URL: GET /api/contratacoes/proposta/{propostaId}
- Response: 200 OK with contract JSON or 404 Not Found

Notes
- The API uses an in-memory repository implementation (InMemoryPropostaRepository) by default. Data is not persisted between restarts.
- Swagger is available in Development environment. If running in Production environment, you may need to enable Swagger or use a different configuration.
- If the API listens on a different port, check the application console output for the actual listening URLs or set ASPNETCORE_URLS or use --urls when running.

Public application service interfaces

The repository exposes application service interfaces used by controllers. Signatures:

- PropostaService.Application.Services.IPropostaService

  Task<Result<Proposta>> CreateAsync(string nomeSegurado, string tipoSeguro, decimal valorCobertura, decimal premioMensal);
  Task<Result<Proposta>> AlterarStatusAsync(Guid propostaId, string novoStatus);
  Task<Proposta?> GetByIdAsync(Guid id);
  Task<IEnumerable<Proposta>> ListAsync();

- ContratacaoService.Application.Services.IContratacaoService

  Task<Contratacao> ContratarAsync(Guid propostaId);
  Task<Contratacao?> GetByIdAsync(Guid id);
  Task<Contratacao?> GetByPropostaIdAsync(Guid propostaId);

Run with Docker

1. Build and start all services with Docker Compose from repository root:

   docker-compose -f docker/docker-compose.yml up --build

2. The services will be available at:
   - PropostaService: http://localhost:5000
   - ContratacaoService: http://localhost:5001
   - RabbitMQ Management UI: http://localhost:15672 (guest/guest)

Database initialization

The docker-compose file mounts SQL init scripts from docker/init-db into each Postgres container. These scripts create the required tables. For production use, switch to EF Core migrations managed by dotnet ef.

Contributing
- Create a branch, open a PR against main. Follow existing code conventions.

License
- See repository license (if any).