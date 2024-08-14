FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["Kvitta/Kvitta.csproj", "Kvitta/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "Kvitta/Kvitta.csproj"
COPY . .
WORKDIR "/src/Kvitta"
RUN dotnet build "Kvitta.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build as development
WORKDIR /src/Kvitta
ENTRYPOINT ["dotnet", "watch", "run", "restore", "--project", "Kvitta.csproj", "--urls", "http://*:8080"]

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Kvitta.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kvitta.dll"]
