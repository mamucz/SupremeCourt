# BASE image s .NET 8 runtime pro spu�t�n� aplikace
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# BUILD image s .NET 8 SDK pro kompilaci
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src

# Zkop�ruj jednotliv� csproj soubory (zrychl� cache p�i rebuildu)
COPY ["SupremeCourt.Presentation/SupremeCourt.Presentation.csproj", "SupremeCourt.Presentation/"]
COPY ["SupremeCourt.Application/SupremeCourt.Application.csproj", "SupremeCourt.Application/"]
COPY ["SupremeCourt.Domain/SupremeCourt.Domain.csproj", "SupremeCourt.Domain/"]
COPY ["SupremeCourt.Infrastructure/SupremeCourt.Infrastructure.csproj", "SupremeCourt.Infrastructure/"]

# Obnoven� z�vislost�
RUN dotnet restore "SupremeCourt.Presentation/SupremeCourt.Presentation.csproj"

# Zkop�ruj zbytek zdrojov�ho k�du
COPY . .

# P�epni se do spr�vn� slo�ky
WORKDIR "/src/SupremeCourt.Presentation"

# Build projektu
RUN dotnet build "SupremeCourt.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publikace aplikace (nap�. pro Release nebo spou�t�n� bez lad�n�)
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SupremeCourt.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# FINAL stage pro spu�t�n� aplikace (i p�i lad�n� ve VS)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# P�i spu�t�n� spust� aplikaci
ENTRYPOINT ["dotnet", "SupremeCourt.Presentation.dll"]
