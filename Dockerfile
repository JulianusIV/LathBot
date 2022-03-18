FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["LathBotFront/LathBotFront.csproj", "LathBotFront/"]
COPY ["LathBotBack/LathBotBack.csproj", "LathBotBack/"]
COPY ["WarnModule/WarnModule.csproj", "WarnModule/"]
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