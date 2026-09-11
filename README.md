# InsureFlow - Proposta Service

This repository contains the Proposta (proposal) microservice for InsureFlow. The API is built with .NET 8 and exposes endpoints to create and list insurance proposals.

Prerequisites
- .NET 8 SDK installed (dotnet --version should show 8.x)
- Git
- (Optional) Visual Studio 2022/2026 or another IDE that supports .NET 8

Build
From the repository root (where InsureFlow.sln is located), run:

- Restore and build the solution:
  dotnet build

Run the API
You can run the API project directly using the dotnet CLI. From the repository root run:

  dotnet run --project src/Services/PropostaService/InsureFlow.PropostaService.Api

When the application starts it will print the listening URLs to the console. By default you can usually access the API at http://localhost:5000 or https://localhost:5001. Swagger UI is enabled in the Development environment at /swagger.

Run tests
To run unit tests:

  dotnet test

or target the test project explicitly:

  dotnet test tests/PropostaService.UnitTests/PropostaService.UnitTests.csproj

API Endpoints
Base route: /api/propostas

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

Notes
- The API uses an in-memory repository implementation (InMemoryPropostaRepository) by default. Data is not persisted between restarts.
- Swagger is available in Development environment. If running in Production environment, you may need to enable Swagger or use a different configuration.
- If the API listens on a different port, check the application console output for the actual listening URLs or set ASPNETCORE_URLS or use --urls when running.

Contributing
- Create a branch, open a PR against main. Follow existing code conventions.

License
- See repository license (if any).