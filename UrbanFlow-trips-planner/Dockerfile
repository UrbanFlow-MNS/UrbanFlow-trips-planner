FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 4008

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UrbanFlow-trips-planner/UrbanFlow-trips-planner.csproj", "./"]
RUN dotnet restore "UrbanFlow-trips-planner.csproj"
COPY . .
WORKDIR "/src/UrbanFlow-trips-planner"
RUN dotnet build "UrbanFlow-trips-planner.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UrbanFlow-trips-planner.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:4008
ENTRYPOINT ["dotnet", "UrbanFlow-trips-planner.dll"]
