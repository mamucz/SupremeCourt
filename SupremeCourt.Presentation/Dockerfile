# 🔹 BASE image s .NET 8 runtime (pro spuštění)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ✅ Přidáme netcat (pro wait-for-it.sh)
RUN apt-get update \
    && apt-get install -y netcat-openbsd \
    && rm -rf /var/lib/apt/lists/*

# 🔹 BUILD image s .NET 8 SDK (pro build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src

# ✅ Kopírování .csproj souborů (lepší cache)
COPY ["SupremeCourt.Presentation/SupremeCourt.Presentation.csproj", "SupremeCourt.Presentation/"]
COPY ["SupremeCourt.Application/SupremeCourt.Application.csproj", "SupremeCourt.Application/"]
COPY ["SupremeCourt.Domain/SupremeCourt.Domain.csproj", "SupremeCourt.Domain/"]
COPY ["SupremeCourt.Infrastructure/SupremeCourt.Infrastructure.csproj", "SupremeCourt.Infrastructure/"]

# ✅ Obnovení závislostí
RUN dotnet restore "SupremeCourt.Presentation/SupremeCourt.Presentation.csproj"

# ✅ Zbytek kódu
COPY . .

# ✅ Přepneme do správné složky a build
WORKDIR "/src/SupremeCourt.Presentation"
RUN dotnet build "SupremeCourt.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 🔹 Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SupremeCourt.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 🔹 FINAL image (pro spuštění)
FROM base AS final
WORKDIR /app

# ✅ Zkopíruj publish výstup
COPY --from=publish /app/publish .

# ✅ Zkopíruj wait-for-it skript
COPY SupremeCourt.Presentation/wait-for-it.sh /wait-for-it.sh
RUN chmod +x /wait-for-it.sh

# ✅ Entry point: počkej na MySQL, pak spusť aplikaci
ENTRYPOINT ["/wait-for-it.sh", "mysql", "3306", "--", "dotnet", "SupremeCourt.Presentation.dll"]
