# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore "BoardGameSchedulerBackend.csproj"

# Copy the rest of the files and publish the app
COPY . ./
RUN dotnet publish "BoardGameSchedulerBackend.csproj" -c Development -o /app/publish

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BoardGameSchedulerBackend.dll"]
