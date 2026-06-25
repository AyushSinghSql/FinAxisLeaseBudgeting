# --- STAGE 1: BUILD ---
# Uses the .NET 10 SDK (now based on Ubuntu 24.04 Noble by default)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build release
COPY . ./
RUN dotnet publish -c Release -o out

# --- STAGE 2: RUNTIME ---
# Uses the .NET 10 ASP.NET Runtime (Ubuntu-based)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

# .NET 10 natively maps your ports using ASPNETCORE_HTTP_PORTS.
# This forces the internal web server to dynamically listen to Render's assigned port.
ENV ASPNETCORE_HTTP_PORTS=${PORT}

# Expose Render's target routing port
EXPOSE 10000

# Remember to change this to your actual project's compiled output dll name!
ENTRYPOINT ["dotnet", "FinAxisLeaseBudgeting.dll"]