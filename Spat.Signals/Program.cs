// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Authorization.Extensions;
using Econolite.Ode.Domain.Asn1.J2735.Extensions;
using Econolite.Ode.Extensions.AspNet;
using Econolite.Ode.Messaging;
using Econolite.Ode.Messaging.Elements;
using Econolite.Ode.Models.Signals;
using Econolite.Ode.Spat.Signals;
using Status.Bsm.Messaging.Extensions;
using Status.Signal.Messaging.Extensions;

await AppBuilder.BuildAndRunWebHostAsync(args, options => { options.Source = "Spat Signal Status"; }, (builderContext, services) =>
{
    services.AddAsn1J2735Service()
        .AddTokenHandler(options =>
        {
            options.Authority = builderContext.Configuration.GetValue("Authentication:Authority", "https://keycloak.cosysdev.com/auth/realms/moundroad")!;
            options.ClientId = builderContext.Configuration.GetValue("Authentication:ClientId", "")!;
            options.ClientSecret = builderContext.Configuration.GetValue("Authentication:ClientSecret", "")!;
        })
        .Configure<KafkaConfigOptions<Guid, OdeSpatData>>(builderContext.Configuration.GetSection("Kafka"))
        .AddSignalStatusProducer(_ => _.SignalStatusTopic = builderContext.Configuration["Topics:SignalStatusTopic"] ?? "signalstatus", _ => { })
        .AddBsmMessageProducer(_ => _.BsmMessageTopic = builderContext.Configuration["Topics:BsmMessageTopic"] ?? "bsmMessage", _ => { })
        .AddTransient<IBuildMessagingConfig<Guid, OdeSpatData>, BuildMessagingConfig<Guid, OdeSpatData>>()
        .AddTransient<IPayloadSpecialist<Guid>, JsonPayloadSpecialist<Guid>>()
        .AddTransient<IPayloadSpecialist<OdeSpatData>, JsonPayloadSpecialist<OdeSpatData>>()
        .AddTransient<IConsumeResultFactory<OdeSpatData>, ConsumeResultFactory<OdeSpatData>>()
        .AddTransient<ISource<OdeSpatData>, Source<OdeSpatData>>()
        .AddTransient<IRawSpatConsumer, RawSpatConsumer>()
        .AddHostedService<Worker>();
});
