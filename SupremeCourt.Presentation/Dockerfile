# BASE image s .NET 8 runtime pro spuštění aplikace
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# 🔧 Přidáme nástroj netcat (nc) – potřebuje ho wait-for-it.sh
RUN apt-get update && apt-get install -y netcat-openbsd

WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# BUILD image s .NET 8 SDK pro kompilaci
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src

# Zkopíruj jednotlivé csproj soubory (zrychlí cache při rebuildu)
COPY ["SupremeCourt.Presentation/SupremeCourt.Presentation.csproj", "SupremeCourt.Presentation/"]
COPY ["SupremeCourt.Application/SupremeCourt.Application.csproj", "SupremeCourt.Application/"]
COPY ["SupremeCourt.Domain/SupremeCourt.Domain.csproj", "SupremeCourt.Domain/"]
COPY ["SupremeCourt.Infrastructure/SupremeCourt.Infrastructure.csproj", "SupremeCourt.Infrastructure/"]

# Obnovení závislostí
RUN dotnet restore "SupremeCourt.Presentation/SupremeCourt.Presentation.csproj"

# Zkopíruj zbytek zdrojového kódu
COPY . .

# Přepni se do správné složky
WORKDIR "/src/SupremeCourt.Presentation"

# Build projektu
RUN dotnet build "SupremeCourt.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publikace aplikace (např. pro Release nebo spouštění bez ladění)
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SupremeCourt.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# FINAL stage pro spuštění aplikace (i při ladění ve VS)
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

COPY SupremeCourt.Presentation/wait-for-it.sh /wait-for-it.sh
RUN chmod +x /wait-for-it.sh

ENTRYPOINT ["/wait-for-it.sh", "mysql", "3306", "--", "dotnet", "SupremeCourt.Presentation.dll"]