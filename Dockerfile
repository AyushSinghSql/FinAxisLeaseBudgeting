# --- STAGE 1: BUILD ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# Copy the project file directly from the root and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build the release version
COPY . ./
RUN dotnet publish -c Release -o out

# --- STAGE 2: RUNTIME ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

# Force .NET 10 to listen to port 10000 on all interfaces
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV ASPNETCORE_HTTP_PORTS=10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "FinAxisLeaseBudgeting.dll"]