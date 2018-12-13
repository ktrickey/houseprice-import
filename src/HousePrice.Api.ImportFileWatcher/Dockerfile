FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /src
COPY ["HousePrice.Api.ImportFileWatcher/HousePrice.Api.ImportFileWatcher.csproj", "HousePrice.Api.ImportFileWatcher/"]
RUN dotnet restore  "HousePrice.Api.ImportFileWatcher/HousePrice.Api.ImportFileWatcher.csproj"
COPY . .
WORKDIR "/src/HousePrice.Api.ImportFileWatcher"
RUN dotnet build "HousePrice.Api.ImportFileWatcher.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "HousePrice.Api.ImportFileWatcher.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "HousePrice.Api.ImportFileWatcher.dll"]
