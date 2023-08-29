# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AgentChatCoordinatorService/AgentChatCoordinatorService.csproj", "AgentChatCoordinatorService/"]
COPY ["Messaging/Messaging.csproj", "Messaging/"]
COPY ["SharedModels/SharedModels.csproj", "SharedModels/"]

RUN dotnet restore "AgentChatCoordinatorService/AgentChatCoordinatorService.csproj"
COPY . .
WORKDIR "/src/AgentChatCoordinatorService"
RUN dotnet build "AgentChatCoordinatorService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AgentChatCoordinatorService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgentChatCoordinatorService.dll"]
