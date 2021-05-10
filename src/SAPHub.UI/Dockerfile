#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/SAPHub.UI/SAPHub.UI.csproj", "src/SAPHub.UI/"]
COPY ["src/SAPHub.UIModule/SAPHub.UIModule.csproj", "src/SAPHub.UIModule/"]
COPY ["src/SAPHub.Common/SAPHub.Common.csproj", "src/SAPHub.Common/"]
COPY ["src/SAPHub.UIModule.Shared/SAPHub.UIModule.Shared.csproj", "src/SAPHub.UIModule.Shared/"]
COPY ["src/SAPHub.UIModule.Client/SAPHub.UIModule.Client.csproj", "src/SAPHub.UIModule.Client/"]
RUN dotnet restore "src/SAPHub.UI/SAPHub.UI.csproj"
COPY . .
WORKDIR "/src/src/SAPHub.UI"
RUN dotnet build "SAPHub.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SAPHub.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SAPHub.UI.dll"]
