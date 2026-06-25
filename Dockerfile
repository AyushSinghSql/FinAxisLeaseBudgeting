# --- STAGE 1: BUILD ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# Copy the project file from its folder and restore dependencies
COPY FinAxisLeaseBudgeting/*.csproj ./FinAxisLeaseBudgeting/
RUN dotnet restore ./FinAxisLeaseBudgeting/FinAxisLeaseBudgeting.csproj

# Copy everything else and build the release version
COPY . ./
WORKDIR /app/FinAxisLeaseBudgeting
RUN dotnet publish -c Release -o /app/out

# --- STAGE 2: RUNTIME ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

# Correctly bind to 0.0.0.0 so Render's port scanner can find it
ENV ASPNETCORE_HTTP_PORTS=0.0.0.0:${PORT}

EXPOSE 10000

ENTRYPOINT ["dotnet", "FinAxisLeaseBudgeting.dll"]