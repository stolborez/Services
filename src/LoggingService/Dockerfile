#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 30100

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["src/LoggingService/LoggingService.csproj", "src/LoggingService/"]
RUN dotnet restore "src/LoggingService/LoggingService.csproj"
COPY . .
WORKDIR "/src/src/LoggingService"
RUN dotnet build "LoggingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LoggingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoggingService.dll"]