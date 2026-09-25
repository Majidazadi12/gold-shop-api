# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the .csproj file and restore dependencies first (for better layer caching)
COPY ["GoldShopAPI.csproj", "./"]
RUN dotnet restore "GoldShopAPI.csproj"

# Copy the rest of the source code and build
COPY . .
RUN dotnet publish "GoldShopAPI.csproj" -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Render provides a PORT environment variable. Kestrel will bind to it.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GoldShopAPI.dll"]