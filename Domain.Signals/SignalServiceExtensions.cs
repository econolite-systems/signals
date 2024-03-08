// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Domain.Signals;

public static class SignalServiceExtensions
{
    public static IServiceCollection AddSignalService(this IServiceCollection services)
    {
        services.AddScoped<ISignalService, SignalService>();

        return services;
    }
}
