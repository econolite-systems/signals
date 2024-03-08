// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Authorization.Extensions;
using Econolite.Ode.Domain.Asn1.J2735.Extensions;
using Econolite.Ode.Extensions.AspNet;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.Signals;
using Econolite.Ode.Monitoring.HealthChecks.Mongo.Extensions;
using Econolite.Ode.Repository.Signals;
using Econolite.Ode.Srm.Signals;

await AppBuilder.BuildAndRunWebHostAsync(args, options => { options.Source = "Srm Signal Status"; }, (builderContext, services) =>
{
    services.AddAsn1J2735Service()
        .AddTokenHandler(options =>
        {
            options.Authority = builderContext.Configuration.GetValue("Authentication:Authority", "https://keycloak.cosysdev.com/auth/realms/moundroad")!;
            options.ClientId = builderContext.Configuration.GetValue("Authentication:ClientId", "")!;
            options.ClientSecret = builderContext.Configuration.GetValue("Authentication:ClientSecret", "")!;
        })
        .Configure<KafkaConfigOptions<Guid, OdeSrmData>>(builderContext.Configuration.GetSection("Kafka"))
        .AddSignalRequestMessageRepo()
        .AddTransient<IBuildMessagingConfig<Guid, OdeSrmData>, BuildMessagingConfig<Guid, OdeSrmData>>()
        .AddTransient<IPayloadSpecialist<Guid>, JsonPayloadSpecialist<Guid>>()
        .AddTransient<IPayloadSpecialist<OdeSrmData>, JsonPayloadSpecialist<OdeSrmData>>()
        .AddTransient<IConsumeResultFactory<Guid, OdeSrmData>, ConsumeResultFactory<OdeSrmData>>()
        .AddTransient<IRawSrmConsumer, RawSrmConsumer>()
        .AddHostedService<Worker>();
}, (_, checksBuilder) => checksBuilder.AddMongoDbHealthCheck());
