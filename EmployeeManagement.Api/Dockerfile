# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["EmployeeManagement.Api/EmployeeManagement.Api.csproj", "EmployeeManagement.Api/"]
COPY ["EmployeeManagement.Application/EmployeeManagement.Application.csproj", "EmployeeManagement.Application/"]
COPY ["EmployeeManagement.Domain/EmployeeManagement.Domain.csproj", "EmployeeManagement.Domain/"]
COPY ["EmployeeManagement.Infrastructure/EmployeeManagement.Infrastructure.csproj", "EmployeeManagement.Infrastructure/"]
RUN dotnet restore "EmployeeManagement.Api/EmployeeManagement.Api.csproj"

# Copy the entire source
COPY . .
WORKDIR "/src/EmployeeManagement.Api"

# Build the app
RUN dotnet build "EmployeeManagement.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "EmployeeManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables for ASP.NET Core
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "EmployeeManagement.Api.dll"]
