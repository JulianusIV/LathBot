FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["nuget.config", "LathBotFront/"]
COPY ["LathBotFront.csproj", "LathBotFront/"]
RUN dotnet restore "LathBotFront/LathBotFront.csproj"
COPY . .
WORKDIR "/src/LathBotFront"
RUN dotnet build "LathBotFront.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LathBotFront.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LathBotFront.dll"]