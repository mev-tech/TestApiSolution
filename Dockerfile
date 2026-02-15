# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution + project files first (for layer caching)
COPY TestApi.sln ./
COPY TestApi/TestApi.csproj TestApi/
COPY TestApi.Tests/TestApi.Tests.csproj TestApi.Tests/

# Copy NuGet configuration files if they exist (these often affect restore)
# (If they don't exist, COPY will fail unless we handle it - so we copy whole context later too)
# We'll do restore after copying full context to be 100% correct on Debian/WSL setups.

# Copy everything (ensures Directory.Packages.props / NuGet.config / global.json are present)
COPY . .

# Restore using the solution (ensures all required packages are fetched)
RUN dotnet restore ./TestApi.sln

# Publish the API
RUN dotnet publish ./TestApi/TestApi.csproj -c Release -o /app/publish --no-restore

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:8000
EXPOSE 8000
ENTRYPOINT ["dotnet", "TestApi.dll"]
