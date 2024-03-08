FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ENV SolutionDir /src
WORKDIR /src
COPY ["Srm.Signals/Srm.Signals.csproj", "Srm.Signals/"]
COPY ["asn1-j2735/Domain.Asn1.J2735/Domain.Asn1.J2735.csproj", "asn1-j2735/Domain.Asn1.J2735/"]
COPY ["asn1-j2735/Asn1J2735/Asn1J2735.csproj", "asn1-j2735/Asn1J2735/"]
COPY ["asn1-j2735/Econolite.J2735_201603/J2735.201603.csproj", "asn1-j2735/Econolite.J2735_201603/"]
COPY ["common/Extensions.AspNet/Extensions.AspNet.csproj", "common/Extensions.AspNet/"]
COPY ["common/Authorization/Authorization.csproj", "common/Authorization/"]
COPY ["common/Extensions/Extensions.csproj", "common/Extensions/"]
COPY ["common/Monitoring.HealthChecks.Kafka/Monitoring.HealthChecks.Kafka.csproj", "common/Monitoring.HealthChecks.Kafka/"]
COPY ["common/Messaging/Messaging.csproj", "common/Messaging/"]
COPY ["common/Monitoring/Monitoring.csproj", "common/Monitoring/"]
COPY ["common/Monitoring.HealthChecks.Mongo/Monitoring.HealthChecks.Mongo.csproj", "common/Monitoring.HealthChecks.Mongo/"]
COPY ["common/Persistence.Mongo/Persistence.Mongo.csproj", "common/Persistence.Mongo/"]
COPY ["common/Persistence.Common/Persistence.Common.csproj", "common/Persistence.Common/"]
COPY ["entities/Models.Entities/Models.Entities.csproj", "entities/Models.Entities/"]
COPY ["config/Config/Config.csproj", "config/Config/"]
COPY ["Models.Signals/Models.Signals.csproj", "Models.Signals/"]
COPY ["status/Status.Signal/Status.Signal.csproj", "status/Status.Signal/"]
COPY ["common/Serialization/Serialization.csproj", "common/Serialization/"]
COPY ["status/Status.Common/Status.Common.csproj", "status/Status.Common/"]
COPY ["Repository.Signals/Repository.Signals.csproj", "Repository.Signals/"]
RUN dotnet restore "Srm.Signals/Srm.Signals.csproj"
COPY . .
WORKDIR "/src/Srm.Signals"
RUN dotnet build "Srm.Signals.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Srm.Signals.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Srm.Signals.dll"]