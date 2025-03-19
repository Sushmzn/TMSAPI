# Use the .NET SDK image for .NET 8.0 to build the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET SDK image for .NET 8.0 to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TaskManagementAPI/TaskManagementAPI.csproj", "TaskManagementAPI/"]
RUN dotnet restore "TaskManagementAPI/TaskManagementAPI.csproj"
COPY . .
WORKDIR "/src/TaskManagementAPI"
RUN dotnet build "TaskManagementAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskManagementAPI.csproj" -c Release -o /app/publish

# Copy the build to the base image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManagementAPI.dll"]
