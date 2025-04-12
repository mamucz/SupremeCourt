# BASE image s .NET 8 runtime pro spuštìní aplikace
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# BUILD image s .NET 8 SDK pro kompilaci
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src

# Zkopíruj jednotlivé csproj soubory (zrychlí cache pøi rebuildu)
COPY ["SupremeCourt.Presentation/SupremeCourt.Presentation.csproj", "SupremeCourt.Presentation/"]
COPY ["SupremeCourt.Application/SupremeCourt.Application.csproj", "SupremeCourt.Application/"]
COPY ["SupremeCourt.Domain/SupremeCourt.Domain.csproj", "SupremeCourt.Domain/"]
COPY ["SupremeCourt.Infrastructure/SupremeCourt.Infrastructure.csproj", "SupremeCourt.Infrastructure/"]

# Obnovení závislostí
RUN dotnet restore "SupremeCourt.Presentation/SupremeCourt.Presentation.csproj"

# Zkopíruj zbytek zdrojového kódu
COPY . .

# Pøepni se do správné složky
WORKDIR "/src/SupremeCourt.Presentation"

# Build projektu
RUN dotnet build "SupremeCourt.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publikace aplikace (napø. pro Release nebo spouštìní bez ladìní)
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SupremeCourt.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# FINAL stage pro spuštìní aplikace (i pøi ladìní ve VS)
FROM base AS final
WORKDIR /app
# ? Add wait-for-it
COPY --from=publish /app/publish . 
COPY SupremeCourt.Presentation/wait-for-it.sh /wait-for-it.sh
RUN chmod +x /wait-for-it.sh

ENTRYPOINT ["/wait-for-it.sh", "mysql", "3306", "--", "dotnet", "SupremeCourt.Presentation.dll"]