FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Collplex.csproj", ""]
RUN dotnet restore "./Collplex.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Collplex.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Collplex.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Collplex.dll"]