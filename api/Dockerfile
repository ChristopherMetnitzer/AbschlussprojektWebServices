# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Abschlussprojekt.csproj", "./"]
RUN dotnet restore "Abschlussprojekt.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/."
RUN dotnet build "Abschlussprojekt.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Abschlussprojekt.csproj" -c Release -o /app/publish

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Abschlussprojekt.dll"]