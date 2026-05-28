FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PoE1ToonAudit/PoE1ToonAudit.csproj", "PoE1ToonAudit/"]
RUN dotnet restore "PoE1ToonAudit/PoE1ToonAudit.csproj"
COPY . .
WORKDIR "/src/PoE1ToonAudit"
RUN dotnet build "./PoE1ToonAudit.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PoE1ToonAudit.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PoE1ToonAudit.dll"]
