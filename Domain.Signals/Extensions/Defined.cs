// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Microsoft.Extensions.DependencyInjection;

namespace Econolite.Ode.Domain.Signals.Extensions;

public static class Defined
{
    public static IServiceCollection AddSignalS2S(this IServiceCollection services, Action<SignalS2SOptions> options)
    {
        services.AddHttpClient();
        services.Configure<SignalS2SOptions>(options);
        services.AddTransient<ISignalS2S, SignalS2S>();
        return services;
    }
}
