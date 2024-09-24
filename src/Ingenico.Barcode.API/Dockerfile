FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copie cada projeto individualmente
COPY ["src/Ingenico.Barcode.API/Ingenico.Barcode.API.csproj", "Ingenico.Barcode.API/"]
COPY ["src/Ingenico.Barcode.Data/Ingenico.Barcode.Data.csproj", "Ingenico.Barcode.Data/"]
COPY ["src/Ingenico.Barcode.Domain/Ingenico.Barcode.Domain.csproj", "Ingenico.Barcode.Domain/"]
COPY ["src/Ingenico.Barcode.Shared/Ingenico.Barcode.Shared.csproj", "Ingenico.Barcode.Shared/"]
COPY ["src/Ingenico.Barcode.IoC/Ingenico.Barcode.IoC.csproj", "Ingenico.Barcode.IoC/"]

# Execute o restore
WORKDIR /src/Ingenico.Barcode.API
RUN dotnet restore "Ingenico.Barcode.API.csproj" --disable-parallel

# Copie o restante do código
COPY . .

# Compilação e publicação
WORKDIR /src/Ingenico.Barcode.API
RUN dotnet build "Ingenico.Barcode.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ingenico.Barcode.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Defina o usuário
USER app

ENTRYPOINT ["dotnet", "Ingenico.Barcode.API.dll"]
