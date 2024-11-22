# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage for ASP.NET Core
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["pointMaster/pointMaster.csproj", "pointMaster/"]
RUN dotnet restore "./pointMaster.csproj"
COPY . . 
WORKDIR "/src"
RUN dotnet build "./pointMaster.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Build stage for Vue.js app
FROM node:18 AS node-build
WORKDIR /js-app
COPY ["js/package.json", "js/package-lock.json*", "./"]
RUN npm install
COPY js/ .
RUN npm run build
RUN ls -la /js-app
RUN ls -la /wwwroot/js/dist

# Publish ASP.NET Core app
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./pointMaster.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Copy Vue.js build output to ASP.NET Core wwwroot
COPY --from=node-build /wwwroot/js/dist /app/publish/wwwroot/js/dist

# Final runtime stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pointMaster.dll"]