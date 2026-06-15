# =================
# Build Stage
# =================

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build

# Copy Solutions
COPY NashAssetManagement.slnx .
COPY Directory.Build.props .
COPY Directory.Packages.props .

# Copy Projects
COPY src/NashAssetManagement.WebAPI/*.csproj src/NashAssetManagement.WebAPI/
COPY src/NashAssetManagement.Application/*.csproj src/NashAssetManagement.Application/
COPY src/NashAssetManagement.Domain/*.csproj src/NashAssetManagement.Domain/
COPY src/NashAssetManagement.Infrastructure/*.csproj src/NashAssetManagement.Infrastructure/
COPY src/NashAssetManagement.Persistence/*.csproj src/NashAssetManagement.Persistence/

# Downloading and Caching Packages
RUN dotnet restore src/NashAssetManagement.WebAPI/NashAssetManagement.WebAPI.csproj
COPY src/ src/

# Build with Release for optimized image
RUN dotnet publish src/NashAssetManagement.WebAPI/NashAssetManagement.WebAPI.csproj -c Release -o /app/publish --no-restore

# =================
# Run Stage
# =================

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Only copy dll or binary files for small image
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT [ "dotnet", "NashAssetManagement.WebAPI.dll" ]