FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SchoolManagementSystem.csproj", "."]
RUN dotnet restore "SchoolManagementSystem.csproj"
COPY . .
RUN dotnet build "SchoolManagementSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SchoolManagementSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SchoolManagementSystem.dll"]
