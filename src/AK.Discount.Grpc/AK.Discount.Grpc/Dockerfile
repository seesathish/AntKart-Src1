FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/AK.Discount.Grpc/AK.Discount.Grpc/AK.Discount.Grpc.csproj", "src/AK.Discount.Grpc/AK.Discount.Grpc/"]
COPY ["src/AK.Discount.Application/AK.Discount.Application/AK.Discount.Application.csproj", "src/AK.Discount.Application/AK.Discount.Application/"]
COPY ["src/AK.Discount.Domain/AK.Discount.Domain/AK.Discount.Domain.csproj", "src/AK.Discount.Domain/AK.Discount.Domain/"]
COPY ["src/AK.Discount.Infrastructure/AK.Discount.Infrastructure/AK.Discount.Infrastructure.csproj", "src/AK.Discount.Infrastructure/AK.Discount.Infrastructure/"]
COPY ["src/AK.BuildingBlocks/AK.BuildingBlocks/AK.BuildingBlocks.csproj", "src/AK.BuildingBlocks/AK.BuildingBlocks/"]
COPY ["nuget.config", "."]
RUN dotnet restore "src/AK.Discount.Grpc/AK.Discount.Grpc/AK.Discount.Grpc.csproj" --configfile nuget.config
COPY . .
WORKDIR "/src/src/AK.Discount.Grpc/AK.Discount.Grpc"
RUN dotnet build "AK.Discount.Grpc.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "AK.Discount.Grpc.csproj" -c Release -o /app/publish /p:UseAppHost=false
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AK.Discount.Grpc.dll"]
