FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["PatientRecoverySystem.MonitoringService/PatientRecoverySystem.MonitoringService.csproj", "PatientRecoverySystem.MonitoringService/"]
COPY ["PatientRecoverySystem.Shared/PatientRecoverySystem.Shared.csproj", "PatientRecoverySystem.Shared/"]
RUN dotnet restore "PatientRecoverySystem.MonitoringService/PatientRecoverySystem.MonitoringService.csproj"
COPY . .
WORKDIR "/src/PatientRecoverySystem.MonitoringService"
RUN dotnet build "PatientRecoverySystem.MonitoringService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PatientRecoverySystem.MonitoringService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PatientRecoverySystem.MonitoringService.dll"]