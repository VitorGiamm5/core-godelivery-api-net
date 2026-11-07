FROM mcr.microsoft.com/dotnet/sdk:10.0.403 AS build

WORKDIR /source

COPY Directory.Packages.props .
COPY global.json .
COPY src/CoreGoDelivery.Api/CoreGoDelivery.Api.csproj src/CoreGoDelivery.Api/
COPY src/CoreGoDelivery.Api/packages.lock.json src/CoreGoDelivery.Api/
COPY src/CoreGoDelivery.Application/CoreGoDelivery.Application.csproj src/CoreGoDelivery.Application/
COPY src/CoreGoDelivery.Application/packages.lock.json src/CoreGoDelivery.Application/
COPY src/CoreGoDelivery.Domain/CoreGoDelivery.Domain.csproj src/CoreGoDelivery.Domain/
COPY src/CoreGoDelivery.Domain/packages.lock.json src/CoreGoDelivery.Domain/
COPY src/CoreGoDelivery.Infrastructure/CoreGoDelivery.Infrastructure.csproj src/CoreGoDelivery.Infrastructure/
COPY src/CoreGoDelivery.Infrastructure/packages.lock.json src/CoreGoDelivery.Infrastructure/

RUN dotnet restore src/CoreGoDelivery.Api/CoreGoDelivery.Api.csproj --locked-mode

COPY src/ src/

RUN dotnet publish src/CoreGoDelivery.Api/CoreGoDelivery.Api.csproj -c Release --no-restore -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0.14 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 9005
USER $APP_UID

ENTRYPOINT ["dotnet", "CoreGoDelivery.Api.dll"]
