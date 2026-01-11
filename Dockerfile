# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy project files
COPY ["Vibe.Test.Web/Vibe.Test.Web.csproj", "Vibe.Test.Web/"]
COPY ["Vibe.Test.Libary/Vibe.Test.Libary.csproj", "Vibe.Test.Libary/"]
COPY ["Vibe.Test.Model/Vibe.Test.Model.csproj", "Vibe.Test.Model/"]
COPY ["Vibe.Test.Servcie/Vibe.Test.Servcie.csproj", "Vibe.Test.Servcie/"]

# Restore dependencies
RUN dotnet restore "Vibe.Test.Web/Vibe.Test.Web.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/Vibe.Test.Web"
RUN dotnet build "Vibe.Test.Web.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Vibe.Test.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vibe.Test.Web.dll"]
