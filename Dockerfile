FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PizzaMauiApp.API/PizzaMauiApp.API.csproj", "PizzaMauiApp.API/"]
COPY ["PizzaMauiApp.API.Core/PizzaMauiApp.API.Core.csproj", "PizzaMauiApp.API.Core/"]
COPY ["PizzaMauiApp.API.Infrastructure/PizzaMauiApp.API.Infrastructure.csproj", "PizzaMauiApp.API.Infrastructure/"]

RUN dotnet restore "PizzaMauiApp.API/PizzaMauiApp.API.csproj"
COPY . .
WORKDIR "/src/PizzaMauiApp.API"
RUN dotnet build "PizzaMauiApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PizzaMauiApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PizzaMauiApp.API.dll"]
