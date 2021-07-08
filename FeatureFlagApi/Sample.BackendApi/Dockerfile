#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Sample.BackendApi/Sample.BackendApi.csproj", "Sample.BackendApi/"]
COPY ["FeatureFlagApi.SDK/FeatureFlagApi.SDK.csproj", "FeatureFlagApi.SDK/"]
COPY ["FeatureFlag.Shared/FeatureFlag.Shared.csproj", "FeatureFlag.Shared/"]
RUN dotnet restore "Sample.BackendApi/Sample.BackendApi.csproj"
COPY . .
WORKDIR "/src/Sample.BackendApi"
RUN dotnet build "Sample.BackendApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.BackendApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.BackendApi.dll"]