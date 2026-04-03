FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos csproj
COPY ["Sistema_de_Taller_Mecanico_de_Citas.UI/Sistema_de_Taller_Mecanico_de_Citas.UI.csproj", "Sistema_de_Taller_Mecanico_de_Citas.UI/"]
COPY ["Sistema_de_Taller_Mecanico_de_Citas.Model/Sistema_de_Taller_Mecanico_de_Citas.Model.csproj", "Sistema_de_Taller_Mecanico_de_Citas.Model/"]

RUN dotnet restore "Sistema_de_Taller_Mecanico_de_Citas.UI/Sistema_de_Taller_Mecanico_de_Citas.UI.csproj"

# Copiar todo y compilar
COPY . .
WORKDIR "/src/Sistema_de_Taller_Mecanico_de_Citas.UI"
RUN dotnet build "Sistema_de_Taller_Mecanico_de_Citas.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sistema_de_Taller_Mecanico_de_Citas.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sistema_de_Taller_Mecanico_de_Citas.UI.dll"]
