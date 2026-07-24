FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["GestionFormations.csproj", "."]
RUN dotnet restore "GestionFormations.csproj"
COPY . .
RUN dotnet build "GestionFormations.csproj" -c Release -o /app/build
RUN dotnet publish "GestionFormations.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /app/data
ENTRYPOINT ["dotnet", "GestionFormations.dll"]
