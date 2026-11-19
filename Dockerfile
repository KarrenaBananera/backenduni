FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src
COPY AngularApp3.Server/AngularApp3.Server.csproj AngularApp3.Server/
RUN dotnet restore AngularApp3.Server/AngularApp3.Server.csproj /p:SkipSpaBuild=true

FROM restore AS build
COPY AngularApp3.Server/ AngularApp3.Server/
RUN dotnet publish AngularApp3.Server/AngularApp3.Server.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:SkipSpaBuild=true \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "AngularApp3.Server.dll"]
