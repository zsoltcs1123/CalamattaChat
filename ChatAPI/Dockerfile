# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ChatAPI/ChatAPI.csproj", "ChatAPI/"]
COPY ["Messaging/Messaging.csproj", "Messaging/"]
COPY ["SharedModels/SharedModels.csproj", "SharedModels/"]

RUN dotnet restore "ChatAPI/ChatAPI.csproj"
COPY . .
WORKDIR "/src/ChatAPI"
RUN dotnet build "ChatAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatAPI.dll"]
