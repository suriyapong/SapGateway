# Use the official .NET 9 SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["SapGateway.csproj", "./"]
RUN dotnet restore "SapGateway.csproj"

COPY ["appsettings.json", "./"]

# Copy the rest of the project files
COPY . .

# Build the app
RUN dotnet publish "SapGateway.csproj" -c Release -o /app/publish

# Use the smaller ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set timezone to Bangkok (UTC+7)
ENV TZ=Asia/Bangkok

# Expose port 8080
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "SapGateway.dll"]
