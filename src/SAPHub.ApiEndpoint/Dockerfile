#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/SAPHub.ApiEndpoint/SAPHub.ApiEndpoint.csproj", "src/SAPHub.ApiEndpoint/"]
COPY ["src/SAPHub.ApiModule/SAPHub.ApiModule.csproj", "src/SAPHub.ApiModule/"]
COPY ["src/SAPHub.Common/SAPHub.Common.csproj", "src/SAPHub.Common/"]
COPY ["src/SAPHub.StateDb/SAPHub.StateDb.csproj", "src/SAPHub.StateDb/"]
RUN dotnet restore "src/SAPHub.ApiEndpoint/SAPHub.ApiEndpoint.csproj"
COPY . .
WORKDIR "/src/src/SAPHub.ApiEndpoint"
RUN dotnet build "SAPHub.ApiEndpoint.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SAPHub.ApiEndpoint.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SAPHub.ApiEndpoint.dll"]
