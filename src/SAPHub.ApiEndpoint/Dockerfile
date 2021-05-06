#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["samples/saphub/SAPHub.ApiEndpoint/SAPHub.ApiEndpoint.csproj", "samples/saphub/SAPHub.ApiEndpoint/"]
COPY ["samples/saphub/SAPHub.ApiModule/SAPHub.ApiModule.csproj", "samples/saphub/SAPHub.ApiModule/"]
COPY ["samples/saphub/SAPHub.Common/SAPHub.Common.csproj", "samples/saphub/SAPHub.Common/"]
RUN dotnet restore "samples/saphub/SAPHub.ApiEndpoint/SAPHub.ApiEndpoint.csproj"
COPY . .
WORKDIR "/src/samples/saphub/SAPHub.ApiEndpoint"
RUN dotnet build "SAPHub.ApiEndpoint.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SAPHub.ApiEndpoint.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SAPHub.ApiEndpoint.dll"]