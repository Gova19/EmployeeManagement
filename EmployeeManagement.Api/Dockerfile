# -------------------------------
# 1️ Base runtime image
# -------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# -------------------------------
# 2️ Build stage
# -------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only the csproj files first (for better build caching)
COPY ["EmployeeManagement.Api/EmployeeManagement.Api.csproj", "EmployeeManagement.Api/"]
COPY ["EmployeeManagement.Application/EmployeeManagement.Application.csproj", "EmployeeManagement.Application/"]
COPY ["EmployeeManagement.Domain/EmployeeManagement.Domain.csproj", "EmployeeManagement.Domain/"]
COPY ["EmployeeManagement.Infrastructure/EmployeeManagement.Infrastructure.csproj", "EmployeeManagement.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "EmployeeManagement.Api/EmployeeManagement.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/EmployeeManagement.Api"
RUN dotnet build "EmployeeManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# -------------------------------
# 3️ Publish stage
# -------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EmployeeManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -------------------------------
# 4️ Final runtime image
# -------------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmployeeManagement.Api.dll"]
