#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install ffmpeg as root and then switch to non-root user
USER root
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    ffmpeg \
    sudo \
    nano \
    coreutils && \
    rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AdminPortalV8.csproj", "."]
RUN dotnet restore "./AdminPortalV8.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./AdminPortalV8.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AdminPortalV8.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdminPortalV8.dll"]