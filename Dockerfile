# ----------------------------
# Stage 1: Build
# ----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore separately (better caching)
COPY ["EcommerceApp.Api/EcommerceApp.Api.csproj", "EcommerceApp.Api/"]
COPY ["EcommerceApp.Application/EcommerceApp.Application.csproj", "EcommerceApp.Application/"]
COPY ["EcommerceApp.Domain/EcommerceApp.Domain.csproj", "EcommerceApp.Domain/"]
COPY ["EcommerceApp.Infrastructure/EcommerceApp.Infrastructure.csproj", "EcommerceApp.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "EcommerceApp.Api/EcommerceApp.Api.csproj"

# Copy everything else
COPY . .

# Build & publish
RUN dotnet publish "EcommerceApp.Api/EcommerceApp.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false   

# ----------------------------
# Stage 2: Runtime
# ----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published app from build stage
COPY --from=build /app/publish .

# Expose port (optional, but good for documentation)
EXPOSE 5000
EXPOSE 5001

# Environment variable for ASP.NET Core
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=false
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
# Tell Kestrel to listen on all interfaces
ENV ASPNETCORE_URLS=http://+:80 

# Entry point
ENTRYPOINT ["dotnet", "EcommerceApp.Api.dll"]

# # Copy entrypoint.sh into /app in the container
# COPY entrypoint.sh /app/entrypoint.sh

# # Make it executable
# RUN chmod +x /app/entrypoint.sh

# # Set the entrypoint
# ENTRYPOINT ["/app/entrypoint.sh"]



