# Stage 1 — build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build

COPY global.json ./
COPY PetRadar.slnx ./
COPY src/ ./src/
COPY tests/ ./tests/

RUN dotnet restore PetRadar.slnx

RUN dotnet publish src/PetRadar.API/PetRadar.API.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

# Stage 2 — runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "PetRadar.API.dll"]
