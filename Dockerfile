# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY Source Source


RUN dotnet publish -c Release -o /app --use-current-runtime --self-contained false Source/BriefingRoomWeb/BriefingRoomWeb.csproj

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
RUN apt-get update && apt-get install -y libgdiplus
WORKDIR /app
COPY --from=build /app .
COPY Database Database
COPY CustomConfigs CustomConfigs
COPY Media Media
COPY Include Include
ENTRYPOINT ["dotnet", "BriefingRoomWeb.dll"]