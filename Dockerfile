FROM mcr.microsoft.com/dotnet/core-nightly/sdk:3.0.100-preview6-alpine3.9
WORKDIR /app

COPY Sora/*.csproj ./
RUN dotnet restore

COPY Sora/ ./
COPY Docker/config.json.docker ./config.json

ENTRYPOINT ["dotnet", "run", "--project Sora", "-c Release"]
