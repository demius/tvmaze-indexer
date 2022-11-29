FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TvMaze.Indexer/TvMaze.Indexer.csproj", "TvMaze.Indexer/"]
RUN dotnet restore "TvMaze.Indexer/TvMaze.Indexer.csproj"
COPY . .
WORKDIR "/src/TvMaze.Indexer"
RUN dotnet build "TvMaze.Indexer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TvMaze.Indexer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TvMaze.Indexer.dll"]
