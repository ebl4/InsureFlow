FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the full source early so restore sees all projects and package references
COPY . .

# Restore the whole solution to ensure all projects' packages are available
RUN dotnet restore "InsureFlow.sln"

WORKDIR /src/src/Services/PropostaService/InsureFlow.PropostaService.Api
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "InsureFlow.PropostaService.Api.dll"]
