// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Extensions.AspNet;
using Econolite.Ode.Monitoring.HealthChecks.Redis.Extensions;
using Econolite.Ode.Repository.Signals;
using Monitoring.AspNet.Metrics;

await AppBuilder.BuildAndRunWebHostAsync(args, options => { options.Source = "Signals Api"; options.IsApi = true; }, (_, services) =>
{
    services.ConfigureRequestMetrics(c =>
    {
        c.RequestCounter = "Requests";
        c.ResponseCounter = "Responses";
    });
    services.AddSignalRepo();
}, (builder, checksBuilder) => checksBuilder.AddRedisHealthCheck(builder.Configuration.GetConnectionString("Redis") ?? throw new NullReferenceException("ConnectionStrings:Redis missing from config.")), app => app.AddRequestMetrics());
