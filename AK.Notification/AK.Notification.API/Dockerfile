FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["AK.Notification/AK.Notification.API/AK.Notification.API.csproj", "AK.Notification/AK.Notification.API/"]
COPY ["AK.Notification/AK.Notification.Application/AK.Notification.Application.csproj", "AK.Notification/AK.Notification.Application/"]
COPY ["AK.Notification/AK.Notification.Domain/AK.Notification.Domain.csproj", "AK.Notification/AK.Notification.Domain/"]
COPY ["AK.Notification/AK.Notification.Infrastructure/AK.Notification.Infrastructure.csproj", "AK.Notification/AK.Notification.Infrastructure/"]
COPY ["AK.BuildingBlocks/AK.BuildingBlocks/AK.BuildingBlocks.csproj", "AK.BuildingBlocks/AK.BuildingBlocks/"]
COPY ["nuget.config", "."]
RUN dotnet restore "AK.Notification/AK.Notification.API/AK.Notification.API.csproj" --configfile nuget.config
COPY . .
WORKDIR "/src/AK.Notification/AK.Notification.API"
RUN dotnet build "AK.Notification.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AK.Notification.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "AK.Notification.API.dll"]
