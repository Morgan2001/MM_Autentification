FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ["src/Authentication.Api/Authentication.Api.csproj", "Authentication.Api/"]
RUN dotnet restore "Authentication.Api/Authentication.Api.csproj"
COPY . .
WORKDIR "src/Authentication.Api"
RUN dotnet build "Authentication.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Authentication.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authentication.Api.dll"]
