FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY ./SandboxService.sln ./SandboxService.sln
COPY ./SandboxService.API/SandboxService.API.csproj ./SandboxService.API/
COPY ./SandboxService.Core/SandboxService.Core.csproj ./SandboxService.Core/
COPY ./SandboxService.Application/SandboxService.Application.csproj ./SandboxService.Application/
COPY ./SandboxService.Persistence/SandboxService.Persistence.csproj ./SandboxService.Persistence/

RUN dotnet restore

COPY ./SandboxService.API/ ./SandboxService.API/
COPY ./SandboxService.Core/ ./SandboxService.Core/
COPY ./SandboxService.Application/ ./SandboxService.Application/
COPY ./SandboxService.Persistence/ ./SandboxService.Persistence/

RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build-env /app/out .

ENTRYPOINT [ "dotnet", "SandboxService.API.dll" ]


