#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/SAPHub.SAPConnector/SAPHub.SAPConnector.csproj", "src/SAPHub.SAPConnector/"]
COPY ["src/SAPHub.MessageBus/SAPHub.MessageBus.csproj", "src/SAPHub.MessageBus/"]
COPY ["src/SAPHub.Common/SAPHub.Common.csproj", "src/SAPHub.Common/"]
COPY ["src/SAPHub.ConnectorModule/SAPHub.ConnectorModule.csproj", "src/SAPHub.ConnectorModule/"]
RUN dotnet restore "src/SAPHub.SAPConnector/SAPHub.SAPConnector.csproj"
COPY . .
WORKDIR "/src/src/SAPHub.SAPConnector"
RUN dotnet build "SAPHub.SAPConnector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SAPHub.SAPConnector.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV LD_LIBRARY_PATH=/app
ENTRYPOINT ["dotnet", "SAPHub.SAPConnector.dll"]
