FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 5246

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CloudImage.csproj", "."]
RUN dotnet restore "CloudImage.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CloudImage.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CloudImage.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CloudImage.dll"]

ENV ASPNETCORE_URLS=http://+:5246
