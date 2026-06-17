FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["TechMove.GLMS.Web/TechMove.GLMS.Web.csproj", "TechMove.GLMS.Web/"]
RUN dotnet restore "TechMove.GLMS.Web/TechMove.GLMS.Web.csproj"
COPY . .
WORKDIR "/src/TechMove.GLMS.Web"
RUN dotnet build "TechMove.GLMS.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TechMove.GLMS.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechMove.GLMS.Web.dll"]