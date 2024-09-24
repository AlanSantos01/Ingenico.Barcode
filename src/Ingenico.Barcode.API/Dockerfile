#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Ingenico.Barcode.API/Ingenico.Barcode.API.csproj", "src/Ingenico.Barcode.API/"]
COPY ["src/Ingenico.Barcode.Data/Ingenico.Barcode.Data.csproj", "src/Ingenico.Barcode.Data/"]
COPY ["src/Ingenico.Barcode.Domain/Ingenico.Barcode.Domain.csproj", "src/Ingenico.Barcode.Domain/"]
COPY ["src/Ingenico.Barcode.Shared/Ingenico.Barcode.Shared.csproj", "src/Ingenico.Barcode.Shared/"]
COPY ["src/Ingenico.Barcode.IoC/Ingenico.Barcode.IoC.csproj", "src/Ingenico.Barcode.IoC/"]
RUN dotnet restore "./src/Ingenico.Barcode.API/Ingenico.Barcode.API.csproj"
COPY . .
WORKDIR "/src/src/Ingenico.Barcode.API"
RUN dotnet build "./Ingenico.Barcode.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ingenico.Barcode.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ingenico.Barcode.API.dll"]