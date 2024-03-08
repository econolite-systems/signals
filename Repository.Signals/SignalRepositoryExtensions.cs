// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Econolite.Ode.Persistence.Mongo;
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Repository.Signals;

public static class SignalRepositoryExtensions
{
    public static IServiceCollection AddSignalRepo(this IServiceCollection services)
    {
        services.AddScoped<ISignalStatusRepository, SignalStatusRepository>();

        return services;
    }
    
    public static IServiceCollection AddSignalStatusMessageRepo(this IServiceCollection services)
    {
        services.AddMongo();
        services.AddScoped<ISignalStatusMessageRepository, SignalStatusMessageRepository>();

        return services;
    }
    
    public static IServiceCollection AddSignalRequestMessageRepo(this IServiceCollection services)
    {
        services.AddMongo();
        services.AddScoped<ISignalRequestMessageRepository, SignalRequestMessageRepository>();

        return services;
    }
}
