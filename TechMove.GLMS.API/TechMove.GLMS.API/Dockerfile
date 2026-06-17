FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TechMove.GLMS.API/TechMove.GLMS.API.csproj", "TechMove.GLMS.API/"]
RUN dotnet restore "TechMove.GLMS.API/TechMove.GLMS.API.csproj"
COPY . .
WORKDIR "/src/TechMove.GLMS.API"
RUN dotnet build "TechMove.GLMS.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TechMove.GLMS.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechMove.GLMS.API.dll"]