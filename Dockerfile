FROM mcr.microsoft.com/dotnet/core/sdk:3.0
WORKDIR /app

COPY Sora/src/Sora/*.csproj ./
RUN dotnet restore

COPY Sora/ ./
COPY Docker/config.json.docker ./config.json

ENTRYPOINT ["dotnet", "run", "--project Sora", "-c Release"]
