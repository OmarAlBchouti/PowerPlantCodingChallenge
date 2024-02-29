# Use the official ASP.NET 8.0 runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Ensure we listen on any IP Address
ENV DOTNET_URLS=http://+:80

# Use the official ASP.NET 8.0 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the entire solution and source code
COPY . .

# Restore NuGet packages for all projects of solution
RUN dotnet build "PowerPlantCodingChallenge.sln" -c $BUILD_CONFIGURATION -o /app/build

# Build the solution
FROM build AS publish
RUN dotnet publish "PowerPlantCodingChallenge.API/PowerPlantCodingChallenge.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Publish the Application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Instruction of command to executed when the container is started
ENTRYPOINT ["dotnet", "PowerPlantCodingChallenge.API.dll"]

