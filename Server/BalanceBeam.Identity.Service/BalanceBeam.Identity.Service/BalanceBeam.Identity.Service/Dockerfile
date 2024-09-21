#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BalanceBeam.Identity.Service/BalanceBeam.Identity.Service.csproj", "BalanceBeam.Identity.Service/"]
COPY ["BalanceBeam.DataLayer/BalanceBeam.Identity.DataAccess.csproj", "BalanceBeam.DataLayer/"]
COPY ["BalanceBeam.Identity.Common/BalanceBeam.Identity.Common.csproj", "BalanceBeam.Identity.Common/"]
COPY ["BalanceBeam.Identity.BusinessLayer/BalanceBeam.Identity.BusinessLogic.csproj", "BalanceBeam.Identity.BusinessLayer/"]
RUN dotnet restore "./BalanceBeam.Identity.Service/BalanceBeam.Identity.Service.csproj"
COPY . .
WORKDIR "/src/BalanceBeam.Identity.Service"
RUN dotnet build "./BalanceBeam.Identity.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BalanceBeam.Identity.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BalanceBeam.Identity.Service.dll"]